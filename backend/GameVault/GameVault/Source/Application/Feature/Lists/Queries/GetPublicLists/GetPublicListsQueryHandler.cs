using GameVault.Source.Application.Dtos.Common;
using GameVault.Source.Application.Dtos.Lists;
using GameVault.Source.Application.Helpers;
using GameVault.Source.Infrastructure.Contexts;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GameVault.Source.Application.Feature.Lists.Queries.GetPublicLists
{
    internal sealed class GetPublicListsQueryHandler
        : IRequestHandler<GetPublicListsQuery, PagedResultDto<GameListSummaryDto>>
    {
        private readonly GameVaultContext _context;

        public GetPublicListsQueryHandler(GameVaultContext context)
        {
            _context = context;
        }

        public async Task<PagedResultDto<GameListSummaryDto>> Handle(GetPublicListsQuery request, CancellationToken cancellationToken)
        {
            // Una lista pública vacía no aporta nada en la portada: solo las que tienen juegos.
            var page = await _context.GameLists
                .AsNoTracking()
                .Where(l => l.IsPublic && l.Items.Any())
                .OrderByDescending(l => l.UpdatedAt ?? l.CreatedAt)
                .ToRows()
                .ToPagedResultAsync(request.Page, request.PageSize, row => row, cancellationToken);

            return new PagedResultDto<GameListSummaryDto>
            {
                Items = await _context.ToSummariesAsync(page.Items, cancellationToken),
                Count = page.Count,
                Page = page.Page,
                PageSize = page.PageSize
            };
        }
    }
}
