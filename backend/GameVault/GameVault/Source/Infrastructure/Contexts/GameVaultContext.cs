using GameVault.Source.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;


namespace GameVault.Source.Infrastructure.Contexts
{
    public class GameVaultContext:IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
    {
        public GameVaultContext(DbContextOptions<GameVaultContext> opt) :base(opt) { }

        public DbSet<Game> Games { get; set; }
        public DbSet<GameGenre> GameGenres { get; set; }
        public DbSet<GameList> GameLists { get; set; }
        public DbSet<GameListItem> GameListItems { get; set; }
        public DbSet<GamePlatform> GamePlatforms { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<Platform> Platforms { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<UserGame> UserGames { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<ReviewComment> ReviewComments { get; set; }
        public DbSet<ReviewLike> ReviewLikes { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {

            base.OnModelCreating(builder);
            #region Tables
            builder.Entity<Game>().ToTable("Game");
            builder.Entity<GameGenre>().ToTable("GameGenre");
            builder.Entity<GameList>().ToTable("GameLists");
            builder.Entity<GameListItem>().ToTable("GameListItems");
            builder.Entity<GamePlatform>().ToTable("GamePlatforms");
            builder.Entity<Genre>().ToTable("Genres");
            builder.Entity<Platform>().ToTable("Platforms");
            builder.Entity<Review>().ToTable("Reviews");
            builder.Entity<UserGame>().ToTable("UserGames");
            builder.Entity<ReviewComment>().ToTable("ReviewComments");
            builder.Entity<ReviewLike>().ToTable("ReviewLikes");
            #endregion

            #region PK 

            builder.Entity<Game>().HasKey(e => e.Id);
            builder.Entity<GameList>().HasKey(e => e.Id);
            builder.Entity<Platform>().HasKey(e => e.Id);
            builder.Entity<Review>().HasKey(e => e.Id);
            builder.Entity<UserGame>().HasKey(e => e.Id);
            builder.Entity<RefreshToken>().HasKey(e => e.Id);
            builder.Entity<ReviewComment>().HasKey(e => e.Id);
            builder.Entity<ReviewLike>().HasKey(like => new
            {
                like.ReviewId,
                like.ApplicationUserId
            });


            //Keys Compuestas
            builder.Entity<GameGenre>().HasKey(gameGenre => new
            {
                gameGenre.GameId,
                gameGenre.GenreId
            });

            builder.Entity<GamePlatform>().HasKey(gp => new
            {
                gp.GameId,
                gp.PlatformId
            });


            #endregion

            #region RelationShips 

            //Ref: ApplicationUser 1-N UserGame

            builder.Entity<ApplicationUser>()
                .HasMany(au => au.UserGames)
                .WithOne(ug => ug.ApplicationUser)
                .HasForeignKey(ug=>ug.ApplicationUserId)
                .OnDelete(DeleteBehavior.Restrict);


            //Ref: ApplicationUser 1-N Reviews

            builder.Entity<ApplicationUser>()
                .HasMany(au => au.Reviews)
                .WithOne(r => r.ApplicationUser)
                .HasForeignKey(r => r.ApplicationUserId)
                .OnDelete(DeleteBehavior.Restrict);

            //Ref: ApplicationUser 1-N Lists

            builder.Entity<ApplicationUser>()
                .HasMany(au => au.GameList)
                .WithOne(gl => gl.ApplicationUser)
                .HasForeignKey(gl => gl.ApplicationUserId)
                .OnDelete(DeleteBehavior.Restrict);


            //Ref: Game 1-N UserGames

            builder.Entity<Game>()
                .HasMany(g => g.UserGames)
                .WithOne(ug => ug.Game)
                .HasForeignKey(ug => ug.GameId)
                .OnDelete(DeleteBehavior.Restrict);

            //Ref: Game 1-N Reviews

            builder.Entity<Game>()
              .HasMany(g => g.Reviews)
              .WithOne(r => r.Game)
              .HasForeignKey(r => r.GameId)
              .OnDelete(DeleteBehavior.Restrict);



            //Ref: Game 1-N GameListItem

            builder.Entity<Game>()
              .HasMany(g => g.GameListItems)
              .WithOne(gli => gli.Game)
              .HasForeignKey(gli => gli.GameId)
              .OnDelete(DeleteBehavior.Restrict);


            //Ref: Game 1-N GameGenres (N-N Genre mediante GameGenre)

            builder.Entity<Game>()
              .HasMany(g => g.GameGenres)
              .WithOne(gg => gg.Game)
              .HasForeignKey(gg => gg.GameId)
              .OnDelete(DeleteBehavior.Restrict);



            //Ref: Game 1-N GamePlatforms ( N-N Platform mediante GamePlatform)

            builder.Entity<Game>()
              .HasMany(g => g.GamePlatforms)
              .WithOne(gp => gp.Game)
              .HasForeignKey(gp => gp.GameId)
              .OnDelete(DeleteBehavior.Restrict);



            //Ref: Genre 1-N GameGenres (Ref N-M Game mediante GameGenre)

            builder.Entity<Genre>()
              .HasMany(g => g.GameGenres)
              .WithOne(gg => gg.Genre)
              .HasForeignKey(gg => gg.GenreId)
              .OnDelete(DeleteBehavior.Restrict);

            //Ref: Platform 1-N Game (Ref N-M Game mediante GamePlatform)

            builder.Entity<Platform>()
              .HasMany(p => p.GamePlatforms)
              .WithOne(gp => gp.Platform)
              .HasForeignKey(gp => gp.PlatformId)
              .OnDelete(DeleteBehavior.Restrict);

            //Ref: GameList 1-N GameListItem
            // Cascade: al borrar una lista se borran sus juegos (los items, no los juegos).

            builder.Entity<GameList>()
              .HasMany(gl => gl.Items)
              .WithOne(i => i.GameList)
              .HasForeignKey(i => i.GameListId)
              .OnDelete(DeleteBehavior.Cascade);

            //Ref: Review 1-N ReviewComment (al borrar la reseña se borran sus comentarios)

            builder.Entity<Review>()
              .HasMany(r => r.Comments)
              .WithOne(c => c.Review)
              .HasForeignKey(c => c.ReviewId)
              .OnDelete(DeleteBehavior.Cascade);

            //Ref: Review 1-N ReviewLike (al borrar la reseña se borran sus me gusta)

            builder.Entity<Review>()
              .HasMany(r => r.Likes)
              .WithOne(l => l.Review)
              .HasForeignKey(l => l.ReviewId)
              .OnDelete(DeleteBehavior.Cascade);

            //Ref: ApplicationUser 1-N ReviewComment / ReviewLike
            // Restrict: SQL Server no permite dos caminos de borrado en cascada
            // hacia la misma tabla (usuario -> reseña -> comentario y usuario -> comentario).

            builder.Entity<ReviewComment>()
              .HasOne(c => c.ApplicationUser)
              .WithMany()
              .HasForeignKey(c => c.ApplicationUserId)
              .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<ReviewLike>()
              .HasOne(l => l.ApplicationUser)
              .WithMany()
              .HasForeignKey(l => l.ApplicationUserId)
              .OnDelete(DeleteBehavior.Restrict);



            builder.Entity<RefreshToken>()
            .HasOne(x => x.User)
            .WithMany(x => x.RefreshTokens)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);



            #endregion

            #region Property Configuration

            builder.Entity<RefreshToken>().Property(x => x.TokenHash)
                .IsRequired()
                .HasMaxLength(128);

            // Mismos límites que los validadores, para que la base de datos no acepte más.
            builder.Entity<Review>().Property(x => x.Title).HasMaxLength(120);
            builder.Entity<Review>().Property(x => x.Content).HasMaxLength(5000);
            builder.Entity<ReviewComment>().Property(x => x.Content).IsRequired().HasMaxLength(1000);
            builder.Entity<GameList>().Property(x => x.Name).IsRequired().HasMaxLength(80);
            builder.Entity<GameList>().Property(x => x.Description).HasMaxLength(500);
            builder.Entity<GameListItem>().Property(x => x.Note).HasMaxLength(300);
            builder.Entity<UserGame>().Property(x => x.PrivateNotes).HasMaxLength(2000);


            #endregion

            #region Settings 
            builder.Entity<RefreshToken>().HasIndex(x => x.TokenHash)
                .IsUnique();

            builder.Entity<Game>()
            .HasIndex(x => x.ExternalId)
            .IsUnique();

            // Géneros y plataformas se buscan por su id de IGDB al guardar un juego.
            builder.Entity<Genre>().HasIndex(x => x.ExternalId).IsUnique();
            builder.Entity<Platform>().HasIndex(x => x.ExternalId).IsUnique();

            // Reglas de negocio que también protege la base de datos:
            // un juego una sola vez en la biblioteca de cada usuario,
            builder.Entity<UserGame>()
                .HasIndex(x => new { x.ApplicationUserId, x.GameId })
                .IsUnique();

            // una reseña por usuario y juego,
            builder.Entity<Review>()
                .HasIndex(x => new { x.ApplicationUserId, x.GameId })
                .IsUnique();

            // y un juego una sola vez en cada lista.
            builder.Entity<GameListItem>()
                .HasIndex(x => new { x.GameListId, x.GameId })
                .IsUnique();

            #endregion
        }




    }
}
