using GameVault.Source.Application.Dtos.Library;
using GameVault.Source.Domain.Enums;
using MediatR;

namespace GameVault.Source.Application.Feature.Library.Queries.GetMyLibrary
{
    /// <summary>Biblioteca del usuario. Sin Status devuelve todos los juegos.</summary>
    public sealed record GetMyLibraryQuery(
        Guid UserId,
        GameStatus? Status = null,
        int Page = 1,
        int PageSize = 24) : IRequest<LibraryPageDto>;
}
