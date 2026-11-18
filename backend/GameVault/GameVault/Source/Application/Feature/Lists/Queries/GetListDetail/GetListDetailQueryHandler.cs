using GameVault.Source.Application.Dtos.Lists;
using GameVault.Source.Application.Exceptions;
using GameVault.Source.Application.Helpers;
using GameVault.Source.Application.Mappings;
using GameVault.Source.Infrastructure.Contexts;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GameVault.Source.Application.Feature.Lists.Queries.GetListDetail
{
    internal sealed class GetListDetailQueryHandler : IRequestHandler<GetListDetailQuery, GameListDetailDto>
    {
        private readonly GameVaultContext _context;

        public GetListDetailQueryHandler(GameVaultContext context)
        {
            _context = context;
        }

        public async Task<GameListDetailDto> Handle(GetListDetailQuery request, CancellationToken cancellationToken)
        {
            var list = await _context.GameLists
                .AsNoTracking()
                .Include(l => l.ApplicationUser)
                .FirstOrDefaultAsync(l => l.Id == request.ListId, cancellationToken);

            var isMine = list is not null && list.ApplicationUserId == request.CurrentUserId;

            // Una lista privada de otro usuario responde 404, igual que si no existiera:
            // así no se revela que existe.
            if (list is null || (!list.IsPublic && !isMine))
            {
                throw ApiErrors.NotFound("La lista no existe.");
            }

            var items = await _context.GameListItems
                .AsNoTracking()
                .Where(i => i.GameListId == list.Id)
                .IncludeCardData()
                .AsSplitQuery()
                .OrderBy(i => i.Position)
                .ToListAsync(cancellationToken);

            return new GameListDetailDto
            {
                Id = list.Id,
                Name = list.Name,
                Description = list.Description,
                IsPublic = list.IsPublic,
                Owner = GameEntityMapper.ToUserSummary(list.ApplicationUser),
                IsMine = isMine,
                Items = items.Select(i => new GameListItemDto
                {
                    Position = i.Position,
                    Note = i.Note,
                    Game = GameEntityMapper.ToCard(i.Game)
                }).ToList(),
                CreatedAt = list.CreatedAt,
                UpdatedAt = list.UpdatedAt
            };
        }
    }
}
