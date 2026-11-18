using GameVault.Source.Application.Dtos.Activity;
using GameVault.Source.Application.Dtos.Common;
using MediatR;

namespace GameVault.Source.Application.Feature.Activity.Queries.GetActivity
{
    /// <summary>Actividad reciente de la comunidad, para la sección "comunidad" del home.</summary>
    public sealed record GetActivityQuery(int Page = 1, int PageSize = 10) : IRequest<PagedResultDto<ActivityDto>>;
}
