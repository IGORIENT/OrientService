using Microsoft.EntityFrameworkCore;
using OrientiringService.Domain.Maps;
using OrientiringService.Domain.Users;

namespace OrientiringService.Infrastructure.Persistence
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users => Set<User>();

        public DbSet<SportMap> Maps => Set<SportMap>();

        public DbSet<ExternalIdentity> ExternalIdentities => Set<ExternalIdentity>();

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) 
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            ConfigureUsers(modelBuilder);
            ConfigureMaps(modelBuilder);
            ConfigureExternalIdentities(modelBuilder);
        }

        private void ConfigureUsers(ModelBuilder modelBuilder)
        {
            var builder = modelBuilder.Entity<User>();

            builder.HasKey(x => x.Id);

            builder.Property(x => x.DisplayName).IsRequired().HasMaxLength(200);

            builder.Property(x => x.Login).HasMaxLength(100);

            builder.Property(x => x.About).HasMaxLength(2000);

            builder.Property(x => x.CreatedAt).IsRequired();

            builder.HasIndex(x => x.Login).IsUnique();
        }

        private void ConfigureMaps(ModelBuilder modelBuilder)
        {
            var builder = modelBuilder.Entity<SportMap>();

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Title).IsRequired().HasMaxLength(300);

            builder.Property(x => x.Description).HasMaxLength(4000);

            builder.Property(x => x.ImagePath).IsRequired().HasMaxLength(1000);

            builder.Property(x => x.SportType).IsRequired();

            builder.Property(x => x.Visability).IsRequired();

            builder.Property(x => x.CreatedAt).IsRequired();

            builder.Property(x => x.UpdatedAt);

            builder.OwnsOne(x => x.Bounds, bounds =>
            {
                bounds.Property(b => b.North).HasColumnName("North").IsRequired();
                bounds.Property(b => b.South).HasColumnName("South").IsRequired();
                bounds.Property(b => b.West).HasColumnName("West").IsRequired();
                bounds.Property(b => b.East).HasColumnName("East").IsRequired();
            });
        }

        private void ConfigureExternalIdentities(ModelBuilder modelBuilder)
        {
            var builder = modelBuilder.Entity<ExternalIdentity>();

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Provider).IsRequired().HasMaxLength(100);

            builder.Property(x => x.Subject).IsRequired().HasMaxLength(200);

            builder.Property(x => x.Email).HasMaxLength(200);

            builder.Property(x => x.LinkedAt).IsRequired();

            builder.HasIndex(x => new {x.Provider, x.Subject}).IsUnique();
        }

    }
}
