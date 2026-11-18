using GameVault.Source.Application.Dtos.Lists;
using GameVault.Source.Application.Helpers;
using GameVault.Source.Infrastructure.Contexts;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GameVault.Source.Application.Feature.Lists.Commands.UpdateList
{
    internal sealed class UpdateListCommandHandler : IRequestHandler<UpdateListCommand, GameListSummaryDto>
    {
        private readonly GameVaultContext _context;

        public UpdateListCommandHandler(GameVaultContext context)
        {
            _context = context;
        }

        public async Task<GameListSummaryDto> Handle(UpdateListCommand request, CancellationToken cancellationToken)
        {
            var list = await _context.GetOwnedListAsync(request.ListId, request.UserId, cancellationToken);

            list.Name = request.Name.Trim();
            list.Description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description.Trim();
            list.IsPublic = request.IsPublic;
            list.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);

            var row = await _context.GameLists
                .AsNoTracking()
                .Where(l => l.Id == list.Id)
                .ToRows()
                .FirstAsync(cancellationToken);

            return (await _context.ToSummariesAsync([row], cancellationToken))[0];
        }
    }
}
