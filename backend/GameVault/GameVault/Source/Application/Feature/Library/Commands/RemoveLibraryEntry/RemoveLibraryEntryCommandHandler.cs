using GameVault.Source.Infrastructure.Contexts;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GameVault.Source.Application.Feature.Library.Commands.RemoveLibraryEntry
{
    internal sealed class RemoveLibraryEntryCommandHandler : IRequestHandler<RemoveLibraryEntryCommand, Unit>
    {
        private readonly GameVaultContext _context;

        public RemoveLibraryEntryCommandHandler(GameVaultContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(RemoveLibraryEntryCommand request, CancellationToken cancellationToken)
        {
            var entry = await _context.UserGames.FirstOrDefaultAsync(
                ug => ug.ApplicationUserId == request.UserId && ug.Game.ExternalId == request.ExternalId,
                cancellationToken);

            if (entry is not null)
            {
                _context.UserGames.Remove(entry);
                await _context.SaveChangesAsync(cancellationToken);
            }

            return Unit.Value;
        }
    }
}
