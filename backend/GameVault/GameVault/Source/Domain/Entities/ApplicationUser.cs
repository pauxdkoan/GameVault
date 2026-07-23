using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ViewEngines;

namespace GameVault.Source.Domain.Entities
{
    public class ApplicationUser:IdentityUser<Guid>
    {
        public string DisplayName { get; set; } = string.Empty;
        public string? Bio { get; set; }
        public string? AvatarUrl { get; set; }
        public string? BannerUrl { get; set; }
        public string? Country { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime? CreatedAt { get; set; }

        //Navigations

        //Ref: 1-N UserGame
        public ICollection<UserGame> UserGames { get; set; } = [];

        //Ref: 1-N Review
        public ICollection<Review> Reviews { get; set; } = [];
        //Ref: 1-N GameList
        public ICollection<GameList> GameList { get; set; } = [];
    }
}
