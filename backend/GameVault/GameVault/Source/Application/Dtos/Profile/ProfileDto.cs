using GameVault.Source.Application.Dtos.Lists;
using GameVault.Source.Application.Dtos.Reviews;

namespace GameVault.Source.Application.Dtos.Profile
{
    /// <summary>Perfil público de un usuario.</summary>
    public sealed class ProfileDto
    {
        public string UserName { get; set; } = string.Empty;

        public string DisplayName { get; set; } = string.Empty;

        public string? Bio { get; set; }

        public string? AvatarUrl { get; set; }

        public string? BannerUrl { get; set; }

        public string? Country { get; set; }

        public DateTime? MemberSince { get; set; }

        /// <summary>true si es el perfil del usuario que lo está viendo.</summary>
        public bool IsMe { get; set; }

        public ProfileStatsDto Stats { get; set; } = new();

        public List<ReviewDto> RecentReviews { get; set; } = [];

        public List<GameListSummaryDto> PublicLists { get; set; } = [];
    }

    public sealed class ProfileStatsDto
    {
        public int GamesInLibrary { get; set; }

        public int Playing { get; set; }

        public int Completed { get; set; }

        public int Wishlist { get; set; }

        public int Reviews { get; set; }

        public int PublicLists { get; set; }
    }
}
