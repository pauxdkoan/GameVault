using GameVault.Source.Application.Dtos.Library;
using GameVault.Source.Application.Helpers;
using GameVault.Source.Application.Mappings;
using GameVault.Source.Infrastructure.Contexts;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GameVault.Source.Application.Feature.Library.Queries.GetMyLibrary
{
    internal sealed class GetMyLibraryQueryHandler : IRequestHandler<GetMyLibraryQuery, LibraryPageDto>
    {
        private readonly GameVaultContext _context;

        public GetMyLibraryQueryHandler(GameVaultContext context)
        {
            _context = context;
        }

        public async Task<LibraryPageDto> Handle(GetMyLibraryQuery request, CancellationToken cancellationToken)
        {
            var userEntries = _context.UserGames
                .AsNoTracking()
                .Where(ug => ug.ApplicationUserId == request.UserId);

            var statusCounts = await userEntries
                .GroupBy(ug => ug.Status)
                .Select(group => new { Status = group.Key, Count = group.Count() })
                .ToDictionaryAsync(x => x.Status, x => x.Count, cancellationToken);

            var filtered = request.Status is null
                ? userEntries
                : userEntries.Where(ug => ug.Status == request.Status);

            // Lo último que el usuario tocó aparece primero.
            var entries = await filtered
                .IncludeCardData()
                .AsSplitQuery()
                .OrderByDescending(ug => ug.UpdatedAt ?? ug.CreatedAt)
                .ToPagedResultAsync(request.Page, request.PageSize, LibraryMapper.ToDto, cancellationToken);

            return new LibraryPageDto
            {
                Entries = entries,
                StatusCounts = statusCounts
            };
        }
    }
}
