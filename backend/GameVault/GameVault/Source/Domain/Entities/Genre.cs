using GameVault.Source.Domain.Common;

namespace GameVault.Source.Domain.Entities
{
    public class Genre:BaseEntity
    {
        public int ExternalId { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Slug { get; set; } = string.Empty;


        // Navigations:

        // Ref N-M Game mediante GameGenre
        public ICollection<GameGenre> GameGenres { get; set; } = [];
    }
}
