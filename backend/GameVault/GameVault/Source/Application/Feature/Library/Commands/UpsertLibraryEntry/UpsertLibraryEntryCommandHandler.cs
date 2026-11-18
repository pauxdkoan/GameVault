using GameVault.Source.Application.Dtos.Library;
using GameVault.Source.Application.Helpers;
using GameVault.Source.Application.Interfaces.Games;
using GameVault.Source.Application.Mappings;
using GameVault.Source.Domain.Entities;
using GameVault.Source.Domain.Enums;
using GameVault.Source.Infrastructure.Contexts;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GameVault.Source.Application.Feature.Library.Commands.UpsertLibraryEntry
{
    internal sealed class UpsertLibraryEntryCommandHandler : IRequestHandler<UpsertLibraryEntryCommand, LibraryEntryDto>
    {
        private readonly GameVaultContext _context;
        private readonly IGameSyncService _gameSyncService;

        public UpsertLibraryEntryCommandHandler(GameVaultContext context, IGameSyncService gameSyncService)
        {
            _context = context;
            _gameSyncService = gameSyncService;
        }

        public async Task<LibraryEntryDto> Handle(UpsertLibraryEntryCommand request, CancellationToken cancellationToken)
        {
            var game = await _gameSyncService.EnsureGameAsync(request.ExternalId, cancellationToken);

            var entry = await _context.UserGames.FirstOrDefaultAsync(
                ug => ug.ApplicationUserId == request.UserId && ug.GameId == game.Id,
                cancellationToken);

            if (entry is null)
            {
                entry = new UserGame { ApplicationUserId = request.UserId, GameId = game.Id };
                _context.UserGames.Add(entry);
            }
            else
            {
                entry.UpdatedAt = DateTime.UtcNow;
            }

            entry.Status = request.Status;
            entry.ProgressPercentage = request.ProgressPercentage;
            entry.HoursPlayed = request.HoursPlayed;
            entry.PersonalRating = request.PersonalRating;
            entry.StartedAt = request.StartedAt;
            entry.CompletedAt = request.CompletedAt;
            entry.PrivateNotes = string.IsNullOrWhiteSpace(request.PrivateNotes) ? null : request.PrivateNotes.Trim();

            // Marcarlo como completado implica 100 % y, si no dice cuándo, hoy.
            if (entry.Status == GameStatus.Completed)
            {
                entry.ProgressPercentage = 100;
                entry.CompletedAt ??= DateOnly.FromDateTime(DateTime.UtcNow);
            }

            await _context.SaveChangesAsync(cancellationToken);

            var saved = await _context.UserGames
                .AsNoTracking()
                .IncludeCardData()
                .AsSplitQuery()
                .FirstAsync(ug => ug.Id == entry.Id, cancellationToken);

            return LibraryMapper.ToDto(saved);
        }
    }
}
