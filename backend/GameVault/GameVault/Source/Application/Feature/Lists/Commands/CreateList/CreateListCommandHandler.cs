using GameVault.Source.Application.Dtos.Lists;
using GameVault.Source.Application.Helpers;
using GameVault.Source.Domain.Entities;
using GameVault.Source.Infrastructure.Contexts;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GameVault.Source.Application.Feature.Lists.Commands.CreateList
{
    internal sealed class CreateListCommandHandler : IRequestHandler<CreateListCommand, GameListSummaryDto>
    {
        private readonly GameVaultContext _context;

        public CreateListCommandHandler(GameVaultContext context)
        {
            _context = context;
        }

        public async Task<GameListSummaryDto> Handle(CreateListCommand request, CancellationToken cancellationToken)
        {
            var list = new GameList
            {
                ApplicationUserId = request.UserId,
                Name = request.Name.Trim(),
                Description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description.Trim(),
                IsPublic = request.IsPublic
            };

            _context.GameLists.Add(list);
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
