using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Company> Companies { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<StudyRecord> StudyRecords { get; set; }
    public DbSet<DailyReport> DailyReports { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User Entity
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("user");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id").HasColumnType("CHAR(36)");
            entity.Property(e => e.Name).HasColumnName("name").IsRequired().HasMaxLength(100);
            entity.Property(e => e.Email).HasColumnName("email").IsRequired().HasMaxLength(255);
            entity.Property(e => e.PasswordHash).HasColumnName("password_hash").IsRequired();
            entity.Property(e => e.Role).HasColumnName("role").IsRequired().HasMaxLength(20);
            entity.Property(e => e.CompanyId).HasColumnName("company_id").HasColumnType("CHAR(36)");
            entity.Property(e => e.IsDeleted).HasColumnName("is_deleted");

            entity.HasOne(e => e.Company)
                .WithMany(c => c.Users)
                .HasForeignKey(e => e.CompanyId);

            entity.HasIndex(e => e.Email).IsUnique();
            entity.HasQueryFilter(e => !e.IsDeleted);
        });

        // Company Entity
        modelBuilder.Entity<Company>(entity =>
        {
            entity.ToTable("company");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id").HasColumnType("CHAR(36)");
            entity.Property(e => e.Name).HasColumnName("name").IsRequired().HasMaxLength(200);
            entity.Property(e => e.IsDeleted).HasColumnName("is_deleted");

            entity.HasQueryFilter(e => !e.IsDeleted);
        });

        // Category Entity
        modelBuilder.Entity<Category>(entity =>
        {
            entity.ToTable("category");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id").HasColumnType("CHAR(36)");
            entity.Property(e => e.UserId).HasColumnName("user_id").HasColumnType("CHAR(36)");
            entity.Property(e => e.Name).HasColumnName("name").IsRequired().HasMaxLength(100);
            entity.Property(e => e.IsDeleted).HasColumnName("is_deleted");

            entity.HasOne(e => e.User)
                .WithMany(u => u.Categories)
                .HasForeignKey(e => e.UserId);

            entity.HasQueryFilter(e => !e.IsDeleted);
        });

        // StudyRecord Entity
        modelBuilder.Entity<StudyRecord>(entity =>
        {
            entity.ToTable("study_record");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id").HasColumnType("CHAR(36)");
            entity.Property(e => e.UserId).HasColumnName("user_id").HasColumnType("CHAR(36)");
            entity.Property(e => e.CategoryId).HasColumnName("category_id").HasColumnType("CHAR(36)");
            entity.Property(e => e.Date).HasColumnName("date").HasColumnType("date");
            entity.Property(e => e.StudyHour).HasColumnName("study_hour").HasColumnType("float");
            entity.Property(e => e.Memo).HasColumnName("memo").HasMaxLength(1000);
            entity.Property(e => e.IsDeleted).HasColumnName("is_deleted");

            entity.HasOne(e => e.User)
                .WithMany(u => u.StudyRecords)
                .HasForeignKey(e => e.UserId);

            entity.HasOne(e => e.Category)
                .WithMany(c => c.StudyRecords)
                .HasForeignKey(e => e.CategoryId);

            entity.HasQueryFilter(e => !e.IsDeleted);
        });

        // DailyReport Entity
        modelBuilder.Entity<DailyReport>(entity =>
        {
            entity.ToTable("daily_report");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id").HasColumnType("CHAR(36)");
            entity.Property(e => e.UserId).HasColumnName("user_id").HasColumnType("CHAR(36)");
            entity.Property(e => e.Date).HasColumnName("date").HasColumnType("date");
            entity.Property(e => e.Goal).HasColumnName("goal").IsRequired();
            entity.Property(e => e.Achieved).HasColumnName("achieved").IsRequired();
            entity.Property(e => e.Struggle).HasColumnName("struggle").IsRequired();
            entity.Property(e => e.Overcame).HasColumnName("overcame").IsRequired();
            entity.Property(e => e.IsDeleted).HasColumnName("is_deleted");

            entity.HasOne(e => e.User)
                .WithMany(u => u.DailyReports)
                .HasForeignKey(e => e.UserId);

            entity.HasIndex(e => new { e.UserId, e.Date }).IsUnique();
            entity.HasQueryFilter(e => !e.IsDeleted);
        });
    }
}
