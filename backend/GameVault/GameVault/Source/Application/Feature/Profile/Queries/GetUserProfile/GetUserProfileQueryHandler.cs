using GameVault.Source.Application.Dtos.Profile;
using GameVault.Source.Application.Exceptions;
using GameVault.Source.Application.Helpers;
using GameVault.Source.Domain.Enums;
using GameVault.Source.Infrastructure.Contexts;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GameVault.Source.Application.Feature.Profile.Queries.GetUserProfile
{
    internal sealed class GetUserProfileQueryHandler : IRequestHandler<GetUserProfileQuery, ProfileDto>
    {
        private const int RecentReviewsCount = 5;
        private const int PublicListsCount = 6;

        private readonly GameVaultContext _context;

        public GetUserProfileQueryHandler(GameVaultContext context)
        {
            _context = context;
        }

        public async Task<ProfileDto> Handle(GetUserProfileQuery request, CancellationToken cancellationToken)
        {
            // Identity guarda el nombre en mayúsculas en NormalizedUserName para buscar sin distinguir mayúsculas.
            var normalizedUserName = request.UserName.Trim().ToUpperInvariant();

            var user = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.NormalizedUserName == normalizedUserName && u.IsActive, cancellationToken)
                ?? throw ApiErrors.NotFound("El usuario no existe.");

            var statusCounts = await _context.UserGames
                .Where(ug => ug.ApplicationUserId == user.Id)
                .GroupBy(ug => ug.Status)
                .Select(group => new { Status = group.Key, Count = group.Count() })
                .ToDictionaryAsync(x => x.Status, x => x.Count, cancellationToken);

            var userReviews = _context.Reviews.AsNoTracking().Where(r => r.ApplicationUserId == user.Id);
            var publicLists = _context.GameLists.AsNoTracking().Where(l => l.ApplicationUserId == user.Id && l.IsPublic);

            var recentReviews = await userReviews
                .ToRows(request.CurrentUserId)
                .OrderByDescending(row => row.Review.CreatedAt)
                .Take(RecentReviewsCount)
                .ToListAsync(cancellationToken);

            var lists = await publicLists
                .OrderByDescending(l => l.UpdatedAt ?? l.CreatedAt)
                .ToRows()
                .Take(PublicListsCount)
                .ToListAsync(cancellationToken);

            return new ProfileDto
            {
                UserName = user.UserName ?? string.Empty,
                DisplayName = string.IsNullOrWhiteSpace(user.DisplayName) ? user.UserName ?? string.Empty : user.DisplayName,
                Bio = user.Bio,
                AvatarUrl = user.AvatarUrl,
                BannerUrl = user.BannerUrl,
                Country = user.Country,
                MemberSince = user.CreatedAt,
                IsMe = user.Id == request.CurrentUserId,
                Stats = new ProfileStatsDto
                {
                    GamesInLibrary = statusCounts.Values.Sum(),
                    Playing = statusCounts.GetValueOrDefault(GameStatus.Playing),
                    Completed = statusCounts.GetValueOrDefault(GameStatus.Completed),
                    Wishlist = statusCounts.GetValueOrDefault(GameStatus.Wishlist),
                    Reviews = await userReviews.CountAsync(r => r.IsVisible, cancellationToken),
                    PublicLists = await publicLists.CountAsync(cancellationToken)
                },
                RecentReviews = recentReviews.Select(row => ReviewQueries.ToDto(row, request.CurrentUserId)).ToList(),
                PublicLists = await _context.ToSummariesAsync(lists, cancellationToken)
            };
        }
    }
}
