namespace GameVault.Source.Domain.Entities
{
    public class GameGenre
    {
        // Navigations:

        // Ref N-1 Game
        public Guid GameId { get; set; }

        public Game Game { get; set; } = null!;


        // Ref N-1 Genre
        public Guid GenreId { get; set; }

        public Genre Genre { get; set; } = null!;
    }
}
