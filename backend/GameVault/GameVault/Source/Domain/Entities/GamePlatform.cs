namespace GameVault.Source.Domain.Entities
{
    public class GamePlatform
    {
        // Navigations:

        // Ref N-1 Game
        public Guid GameId { get; set; }

        public Game Game { get; set; } = null!;


        // Ref N-1 Platform
        public Guid PlatformId { get; set; }

        public Platform Platform { get; set; } = null!;
    }
}
