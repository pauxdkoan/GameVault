using GameVault.Source.Application.Dtos.Lists;
using GameVault.Source.Application.Helpers;
using GameVault.Source.Infrastructure.Contexts;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GameVault.Source.Application.Feature.Lists.Queries.GetMyLists
{
    internal sealed class GetMyListsQueryHandler : IRequestHandler<GetMyListsQuery, List<GameListSummaryDto>>
    {
        private readonly GameVaultContext _context;

        public GetMyListsQueryHandler(GameVaultContext context)
        {
            _context = context;
        }

        public async Task<List<GameListSummaryDto>> Handle(GetMyListsQuery request, CancellationToken cancellationToken)
        {
            var rows = await _context.GameLists
                .AsNoTracking()
                .Where(l => l.ApplicationUserId == request.UserId)
                .OrderByDescending(l => l.UpdatedAt ?? l.CreatedAt)
                .ToRows()
                .ToListAsync(cancellationToken);

            return await _context.ToSummariesAsync(rows, cancellationToken);
        }
    }
}
