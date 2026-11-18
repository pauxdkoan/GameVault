using GameVault.Source.Application.Dtos.Lists;
using GameVault.Source.Application.Exceptions;
using GameVault.Source.Application.Helpers;
using GameVault.Source.Application.Interfaces.Games;
using GameVault.Source.Application.Mappings;
using GameVault.Source.Domain.Entities;
using GameVault.Source.Infrastructure.Contexts;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GameVault.Source.Application.Feature.Lists.Commands.AddGameToList
{
    internal sealed class AddGameToListCommandHandler : IRequestHandler<AddGameToListCommand, GameListItemDto>
    {
        private readonly GameVaultContext _context;
        private readonly IGameSyncService _gameSyncService;

        public AddGameToListCommandHandler(GameVaultContext context, IGameSyncService gameSyncService)
        {
            _context = context;
            _gameSyncService = gameSyncService;
        }

        public async Task<GameListItemDto> Handle(AddGameToListCommand request, CancellationToken cancellationToken)
        {
            var list = await _context.GetOwnedListAsync(request.ListId, request.UserId, cancellationToken);
            var game = await _gameSyncService.EnsureGameAsync(request.ExternalId, cancellationToken);

            var items = _context.GameListItems.Where(i => i.GameListId == list.Id);

            if (await items.AnyAsync(i => i.GameId == game.Id, cancellationToken))
            {
                throw ApiErrors.Conflict("Este juego ya está en la lista.");
            }

            // Position empieza en 1 y el nuevo juego va al final.
            var lastPosition = await items.MaxAsync(i => (int?)i.Position, cancellationToken) ?? 0;

            var item = new GameListItem
            {
                GameListId = list.Id,
                GameId = game.Id,
                Position = lastPosition + 1,
                Note = string.IsNullOrWhiteSpace(request.Note) ? null : request.Note.Trim()
            };

            _context.GameListItems.Add(item);
            list.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync(cancellationToken);

            var saved = await _context.GameListItems
                .AsNoTracking()
                .IncludeCardData()
                .AsSplitQuery()
                .FirstAsync(i => i.Id == item.Id, cancellationToken);

            return new GameListItemDto
            {
                Position = saved.Position,
                Note = saved.Note,
                Game = GameEntityMapper.ToCard(saved.Game)
            };
        }
    }
}
