using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace OscarBot.Models
{
    public partial class BotDbContext : DbContext
    {
        public BotDbContext()
        {
        }

        public BotDbContext(DbContextOptions<BotDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Movie> Movie { get; set; }
        public virtual DbSet<User> User { get; set; }
        public virtual DbSet<Event> Event { get; set; }
        public virtual DbSet<EventMovie> EventMovie { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                throw new Exception("Not configured");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Movie>(entity =>
            {
                entity.Property(e => e.Id).IsRequired();

                entity.Property(e => e.Title)
                    .IsRequired();

                entity.Property(e => e.ServerId).HasMaxLength(100);

                entity.Property(e => e.Picked)
                    .IsRequired();

                entity.Property(e => e.Watched)
                    .IsRequired();
            });

            modelBuilder.Entity<Event>(entity =>
            {
                entity.Property(e => e.Title).HasMaxLength(200);
            });

            modelBuilder.Entity<EventMovie>(entity =>
            {
                entity.HasKey(e => new { e.EventId, e.MovieId });

                entity.Property(e => e.EventId).IsRequired();
                entity.Property(e => e.MovieId).IsRequired();

                entity.HasOne(x => x.Movie).WithMany(x => x.EventMovies).HasForeignKey(e => e.MovieId).OnDelete(DeleteBehavior.ClientCascade);
                entity.HasOne(x => x.Event).WithMany(x => x.EventMovies).HasForeignKey(e => e.EventId).OnDelete(DeleteBehavior.ClientCascade);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
