namespace ApiSpaceShooter.Infrastructure.Persistence;

using ApiSpaceShooter.Domain.Entities;
using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Score> Scores { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Score>(entity =>
        {
            entity.ToTable("Score", "dbo");

            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id)
                .UseIdentityColumn();

            entity.Property(e => e.Alias)
                .IsRequired()
                .HasMaxLength(30)
                .HasColumnType("nvarchar(30)");

            entity.Property(e => e.Points)
                .IsRequired()
                .HasColumnType("int");

            entity.Property(e => e.MaxCombo)
                .HasColumnType("int");

            entity.Property(e => e.DurationSec)
                .HasColumnType("int");

            entity.Property(e => e.Metadata)
                .HasMaxLength(400)
                .HasColumnType("nvarchar(400)");

            entity.Property(e => e.CreatedAt)
                .IsRequired()
                .HasColumnType("datetime2")
                .HasDefaultValueSql("SYSUTCDATETIME()");

            // Índices
            entity.HasIndex(e => e.Points, "IX_Score_Points_DESC")
                .IsDescending();

            entity.HasIndex(e => new { e.Alias, e.CreatedAt }, "IX_Score_Alias_CreatedAt")
                .IsDescending(false, true);

            // Constraints
            entity.HasCheckConstraint("CK_Score_Points", "Points >= 0");
            entity.HasCheckConstraint("CK_Score_MaxCombo", "MaxCombo IS NULL OR MaxCombo >= 0");
            entity.HasCheckConstraint("CK_Score_DurationSec", "DurationSec IS NULL OR DurationSec >= 0");
        });
    }
}