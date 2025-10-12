using Microsoft.EntityFrameworkCore;
using SubscriptionService.Models;

namespace SubscriptionService.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Service> Services { get; set; }
    public DbSet<Token> Tokens { get; set; }
    public DbSet<Subscription> Subscriptions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Service>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.ServiceId).IsUnique();
            entity.Property(e => e.ServiceId).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Password).HasMaxLength(255).IsRequired();
        });

        modelBuilder.Entity<Token>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.TokenId).IsUnique();
            entity.Property(e => e.TokenId).HasMaxLength(100).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.ExpiresAt).IsRequired();
        });

        modelBuilder.Entity<Subscription>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.SubscriptionId).IsUnique();
            entity.Property(e => e.SubscriptionId).HasMaxLength(100).IsRequired();
            entity.Property(e => e.PhoneNumber).HasMaxLength(50).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.UpdatedAt).IsRequired();
            entity.Property(e => e.IsActive).IsRequired();
        });
    }
}
