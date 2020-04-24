using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace MoviesAPI.Models
{
    public partial class movies_apiContext : IdentityDbContext      //DbContext
    {
        public movies_apiContext()
        {
        }

        public movies_apiContext(DbContextOptions<movies_apiContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Genre> Genres  { get; set; }
        public virtual DbSet<Person> People  { get; set; }
        public virtual DbSet<Movie> Movies  { get; set; }
        public virtual DbSet<MoviesGenres> MoviesGenres  { get; set; }
        public virtual DbSet<MoviesActors> MoviesActors  { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Genre>(entity =>
            {
                entity.ToTable("genre");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar(50)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");
            });
            modelBuilder.Entity<MoviesGenres>().HasKey(x => new { x.GenreId, x.MovieId });
            modelBuilder.Entity<MoviesActors>().HasKey(x => new { x.PersonId, x.MovieId });

            SeedData(modelBuilder);

            OnModelCreatingPartial(modelBuilder);

            base.OnModelCreating(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            var adventure = new Genre() { Id = 9, Name = "Adventure" };
            var animation = new Genre() { Id = 8, Name = "Animation" };
            var romance = new Genre() { Id = 7, Name = "Romance" };

            modelBuilder.Entity<Genre>()
                .HasData(new List<Genre>
                {
                    adventure, animation, romance
                });

            var jimCarrey = new Person() { Id = 4, Name = "Jim Carrey", DateOfBirth = new DateTime(1962, 01, 17) };
            var robertDowney = new Person() { Id = 5, Name = "Robert Downey Jr.", DateOfBirth = new DateTime(1965, 4, 4) };
            var chrisEvans = new Person() { Id = 6, Name = "Chris Evans", DateOfBirth = new DateTime(1981, 06, 13) };

            modelBuilder.Entity<Person>()
                .HasData(new List<Person>
                {
                    jimCarrey, robertDowney, chrisEvans
                });

            var endgame = new Movie()
            {
                Id = 5,
                Title = "Avengers: Endgame",
                InTheaters = true,
                ReleaseDate = new DateTime(2019, 04, 26)
            };

            var iw = new Movie()
            {
                Id = 6,
                Title = "Avengers: Infinity Wars",
                InTheaters = false,
                ReleaseDate = new DateTime(2019, 04, 26)
            };

            var sonic = new Movie()
            {
                Id = 7,
                Title = "Sonic the Hedgehog",
                InTheaters = false,
                ReleaseDate = new DateTime(2020, 02, 28)
            };
            var emma = new Movie()
            {
                Id = 8,
                Title = "Emma",
                InTheaters = false,
                ReleaseDate = new DateTime(2020, 02, 21)
            };
            var greed = new Movie()
            {
                Id = 9,
                Title = "Greed",
                InTheaters = false,
                ReleaseDate = new DateTime(2020, 02, 21)
            };

            modelBuilder.Entity<Movie>()
                .HasData(new List<Movie>
                {
                    endgame, iw, sonic, emma, greed
                });

            modelBuilder.Entity<MoviesGenres>().HasData(
                new List<MoviesGenres>()
                {

                    new MoviesGenres(){MovieId = endgame.Id, GenreId = adventure.Id},

                    new MoviesGenres(){MovieId = iw.Id, GenreId = adventure.Id},

                    new MoviesGenres(){MovieId = emma.Id, GenreId = romance.Id},

                    new MoviesGenres(){MovieId = greed.Id, GenreId = romance.Id},
                });

            modelBuilder.Entity<MoviesActors>().HasData(
                new List<MoviesActors>()
                {
                    new MoviesActors(){MovieId = endgame.Id, PersonId = robertDowney.Id, Character = "Tony Stark", Order = 1},
                    new MoviesActors(){MovieId = endgame.Id, PersonId = chrisEvans.Id, Character = "Steve Rogers", Order = 2},
                    new MoviesActors(){MovieId = iw.Id, PersonId = robertDowney.Id, Character = "Tony Stark", Order = 1},
                    new MoviesActors(){MovieId = iw.Id, PersonId = chrisEvans.Id, Character = "Steve Rogers", Order = 2},
                    new MoviesActors(){MovieId = sonic.Id, PersonId = jimCarrey.Id, Character = "Dr. Ivo Robotnik", Order = 1}
                });
        }
        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);

        
    }
}
