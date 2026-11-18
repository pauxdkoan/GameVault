using GameVault.Source.Application.Dtos.Library;
using GameVault.Source.Domain.Enums;
using MediatR;

namespace GameVault.Source.Application.Feature.Library.Commands.UpsertLibraryEntry
{
    /// <summary>
    /// Añade un juego a la biblioteca o actualiza el que ya está ("upsert").
    /// Un solo endpoint (PUT) sirve para las dos cosas: el frontend no necesita
    /// saber si el juego ya estaba.
    /// </summary>
    public sealed record UpsertLibraryEntryCommand(
        Guid UserId,
        int ExternalId,
        GameStatus Status,
        int ProgressPercentage = 0,
        decimal? HoursPlayed = null,
        int? PersonalRating = null,
        DateOnly? StartedAt = null,
        DateOnly? CompletedAt = null,
        string? PrivateNotes = null) : IRequest<LibraryEntryDto>;

    /// <summary>Cuerpo del PUT. El usuario sale del token y el juego de la URL.</summary>
    public sealed record UpsertLibraryEntryRequest(
        GameStatus Status,
        int ProgressPercentage = 0,
        decimal? HoursPlayed = null,
        int? PersonalRating = null,
        DateOnly? StartedAt = null,
        DateOnly? CompletedAt = null,
        string? PrivateNotes = null);
}
