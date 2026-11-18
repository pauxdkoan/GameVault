using GameVault.Source.Application.Helpers;
using GameVault.Source.Infrastructure.Contexts;
using MediatR;

namespace GameVault.Source.Application.Feature.Lists.Commands.DeleteList
{
    internal sealed class DeleteListCommandHandler : IRequestHandler<DeleteListCommand, Unit>
    {
        private readonly GameVaultContext _context;

        public DeleteListCommandHandler(GameVaultContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(DeleteListCommand request, CancellationToken cancellationToken)
        {
            var list = await _context.GetOwnedListAsync(request.ListId, request.UserId, cancellationToken);

            _context.GameLists.Remove(list);
            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
