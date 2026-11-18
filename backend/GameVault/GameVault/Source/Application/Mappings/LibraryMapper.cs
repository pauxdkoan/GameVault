using GameVault.Source.Application.Dtos.Library;
using GameVault.Source.Domain.Entities;

namespace GameVault.Source.Application.Mappings
{
    internal static class LibraryMapper
    {
        /// <summary>Requiere UserGame.Game con géneros y plataformas cargados.</summary>
        public static LibraryEntryDto ToDto(UserGame entry)
        {
            return new LibraryEntryDto
            {
                Game = GameEntityMapper.ToCard(entry.Game),
                Status = entry.Status,
                ProgressPercentage = entry.ProgressPercentage,
                HoursPlayed = entry.HoursPlayed,
                PersonalRating = entry.PersonalRating,
                StartedAt = entry.StartedAt,
                CompletedAt = entry.CompletedAt,
                PrivateNotes = entry.PrivateNotes,
                AddedAt = entry.CreatedAt,
                UpdatedAt = entry.UpdatedAt
            };
        }
    }
}
