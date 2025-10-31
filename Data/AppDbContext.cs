using Microsoft.EntityFrameworkCore;
using MovieMatch.Api.Models;

namespace MovieMatch.Api.Data
{
    public sealed class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Movie> Movies => Set<Movie>();
        public DbSet<User> Users => Set<User>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            // Seed a few movies for demo
            modelBuilder.Entity<Movie>().HasData(
                new Movie { Id = 1, Title = "Inception", Genre = "Sci-Fi", Year = 2010, Rating = 8.8 },
                new Movie { Id = 2, Title = "The Dark Knight", Genre = "Action", Year = 2008, Rating = 9.0 },
                new Movie { Id = 3, Title = "Interstellar", Genre = "Sci-Fi", Year = 2014, Rating = 8.6 }
            );
        }
    }
}

