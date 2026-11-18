using GameVault.Source.Application.Dtos.Activity;
using GameVault.Source.Application.Dtos.Common;
using GameVault.Source.Application.Helpers;
using GameVault.Source.Application.Mappings;
using GameVault.Source.Domain.Enums;
using GameVault.Source.Infrastructure.Contexts;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GameVault.Source.Application.Feature.Activity.Queries.GetActivity
{
    /// <summary>
    /// No hay tabla de actividad: se arma juntando tres consultas
    /// (reseñas, juegos completados y listas públicas) y ordenando por fecha.
    /// Para la página N basta con traer N * tamaño de cada una y cortar en memoria.
    /// </summary>
    internal sealed class GetActivityQueryHandler : IRequestHandler<GetActivityQuery, PagedResultDto<ActivityDto>>
    {
        private const int ExcerptLength = 160;

        private readonly GameVaultContext _context;

        public GetActivityQueryHandler(GameVaultContext context)
        {
            _context = context;
        }

        public async Task<PagedResultDto<ActivityDto>> Handle(GetActivityQuery request, CancellationToken cancellationToken)
        {
            var (page, pageSize) = Paging.Normalize(request.Page, request.PageSize);
            var take = page * pageSize;

            var reviews = _context.Reviews.AsNoTracking().Where(r => r.IsVisible);
            var completed = _context.UserGames.AsNoTracking().Where(ug => ug.Status == GameStatus.Completed);
            var lists = _context.GameLists.AsNoTracking().Where(l => l.IsPublic && l.Items.Any());

            var reviewRows = await reviews
                .ToRows(currentUserId: null)
                .OrderByDescending(row => row.Review.CreatedAt)
                .Take(take)
                .ToListAsync(cancellationToken);

            var completedRows = await completed
                .Include(ug => ug.ApplicationUser)
                .Include(ug => ug.Game)
                .OrderByDescending(ug => ug.UpdatedAt ?? ug.CreatedAt)
                .Take(take)
                .ToListAsync(cancellationToken);

            var listRows = await lists
                .OrderByDescending(l => l.CreatedAt)
                .ToRows()
                .Take(take)
                .ToListAsync(cancellationToken);

            var activity = reviewRows.Select(row => new ActivityDto
                {
                    Type = ActivityType.Review,
                    User = GameEntityMapper.ToUserSummary(row.Author),
                    OccurredAt = row.Review.CreatedAt,
                    GameExternalId = row.Game.ExternalId,
                    GameName = row.Game.Name,
                    GameCoverImage = row.Game.CoverUrl,
                    ReviewId = row.Review.Id,
                    Rating = row.Review.Rating,
                    Excerpt = row.Review.ContainsSpoilers ? null : Excerpt(row.Review.Content),
                    LikeCount = row.LikeCount,
                    CommentCount = row.CommentCount
                })
                .Concat(completedRows.Select(ug => new ActivityDto
                {
                    Type = ActivityType.Completed,
                    User = GameEntityMapper.ToUserSummary(ug.ApplicationUser),
                    OccurredAt = ug.UpdatedAt ?? ug.CreatedAt,
                    GameExternalId = ug.Game.ExternalId,
                    GameName = ug.Game.Name,
                    GameCoverImage = ug.Game.CoverUrl
                }))
                .Concat(listRows.Select(row => new ActivityDto
                {
                    Type = ActivityType.ListCreated,
                    User = GameEntityMapper.ToUserSummary(row.Owner),
                    OccurredAt = row.List.CreatedAt,
                    ListId = row.List.Id,
                    ListName = row.List.Name,
                    ListGameCount = row.GameCount
                }))
                .OrderByDescending(item => item.OccurredAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var count =
                await reviews.CountAsync(cancellationToken) +
                await completed.CountAsync(cancellationToken) +
                await lists.CountAsync(cancellationToken);

            return new PagedResultDto<ActivityDto>
            {
                Items = activity,
                Count = count,
                Page = page,
                PageSize = pageSize
            };
        }

        private static string Excerpt(string content)
        {
            var text = content.ReplaceLineEndings(" ").Trim();
            return text.Length <= ExcerptLength ? text : text[..ExcerptLength].TrimEnd() + "…";
        }
    }
}
