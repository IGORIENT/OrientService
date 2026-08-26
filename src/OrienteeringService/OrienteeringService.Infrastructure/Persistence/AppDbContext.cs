using Microsoft.EntityFrameworkCore;
using OrienteeringService.Domain.Maps;
using OrienteeringService.Domain.Users;

namespace OrienteeringService.Infrastructure.Persistence
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        internal const string ExternalIdentityIndexName = "ux_external_identities_issuer_subject";

        public DbSet<User> Users => Set<User>();

        public DbSet<SportMap> Maps => Set<SportMap>();

        public DbSet<ExternalIdentity> ExternalIdentities => Set<ExternalIdentity>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            ConfigureUsers(modelBuilder);
            ConfigureMaps(modelBuilder);
            ConfigureExternalIdentities(modelBuilder);
        }

        private static void ConfigureUsers(ModelBuilder modelBuilder)
        {
            var builder = modelBuilder.Entity<User>();

            builder.HasKey(user => user.Id);

            builder.Property(user => user.DisplayName).IsRequired().HasMaxLength(200);
            builder.Property(user => user.Login).IsRequired().HasMaxLength(100);
            builder.Property(user => user.About).HasMaxLength(2000);
            builder.Property(user => user.CreatedAt).IsRequired();

            builder.HasIndex(user => user.Login).IsUnique();
        }

        private static void ConfigureMaps(ModelBuilder modelBuilder)
        {
            var builder = modelBuilder.Entity<SportMap>();

            builder.HasKey(map => map.Id);

            builder.Property(map => map.Title).IsRequired().HasMaxLength(300);
            builder.Property(map => map.Description).HasMaxLength(4000);
            builder.Property(map => map.ImagePath).IsRequired().HasMaxLength(1000);
            builder.Property(map => map.SportType).IsRequired();
            builder.Property(map => map.Visibility).IsRequired();
            builder.Property(map => map.CreatedAt).IsRequired();
            builder.Property(map => map.UpdatedAt).IsRequired();

            builder
                .HasOne<User>()
                .WithMany()
                .HasForeignKey(map => map.OwnerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.OwnsOne(map => map.Bounds, bounds =>
            {
                bounds.Property(value => value.North).HasColumnName("North").IsRequired();
                bounds.Property(value => value.South).HasColumnName("South").IsRequired();
                bounds.Property(value => value.West).HasColumnName("West").IsRequired();
                bounds.Property(value => value.East).HasColumnName("East").IsRequired();
            });
        }

        private static void ConfigureExternalIdentities(ModelBuilder modelBuilder)
        {
            var builder = modelBuilder.Entity<ExternalIdentity>();

            builder.HasKey(identity => identity.Id);

            builder.Property(identity => identity.Issuer).IsRequired().HasMaxLength(500);
            builder.Property(identity => identity.Subject).IsRequired().HasMaxLength(500);
            builder.Property(identity => identity.Email).HasMaxLength(320);
            builder.Property(identity => identity.LinkedAt).IsRequired();

            builder
                .HasOne<User>()
                .WithMany()
                .HasForeignKey(identity => identity.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder
                .HasIndex(identity => new { identity.Issuer, identity.Subject })
                .IsUnique()
                .HasDatabaseName(ExternalIdentityIndexName);
        }
    }
}
