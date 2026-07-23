using GameVault.Source.Domain.Common;

namespace GameVault.Source.Domain.Entities
{
    public class Platform : BaseEntity
    {
        public int ExternalId { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Slug { get; set; } = string.Empty;


        // Navigations:

        // Ref N-N Game mediante GamePlatform
        public ICollection<GamePlatform> GamePlatforms { get; set; } = [];
    }
}
