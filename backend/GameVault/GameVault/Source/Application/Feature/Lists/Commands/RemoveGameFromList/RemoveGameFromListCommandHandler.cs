using GameVault.Source.Application.Helpers;
using GameVault.Source.Infrastructure.Contexts;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GameVault.Source.Application.Feature.Lists.Commands.RemoveGameFromList
{
    internal sealed class RemoveGameFromListCommandHandler : IRequestHandler<RemoveGameFromListCommand, Unit>
    {
        private readonly GameVaultContext _context;

        public RemoveGameFromListCommandHandler(GameVaultContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(RemoveGameFromListCommand request, CancellationToken cancellationToken)
        {
            var list = await _context.GetOwnedListAsync(request.ListId, request.UserId, cancellationToken);

            var items = await _context.GameListItems
                .Include(i => i.Game)
                .Where(i => i.GameListId == list.Id)
                .OrderBy(i => i.Position)
                .ToListAsync(cancellationToken);

            var toRemove = items.FirstOrDefault(i => i.Game.ExternalId == request.ExternalId);

            if (toRemove is null)
            {
                return Unit.Value;
            }

            _context.GameListItems.Remove(toRemove);

            // Recolocar: 1, 2, 4, 5 pasa a ser 1, 2, 3, 4.
            var position = 1;
            foreach (var item in items.Where(i => i != toRemove))
            {
                item.Position = position++;
            }

            list.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
