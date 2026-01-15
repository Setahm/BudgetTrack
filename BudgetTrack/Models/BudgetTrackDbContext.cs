using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace BudgetTrack.Models;

public partial class BudgetTrackDbContext : DbContext
{
    public BudgetTrackDbContext()
    {
    }

    public BudgetTrackDbContext(DbContextOptions<BudgetTrackDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Company> Companies { get; set; }

    public virtual DbSet<Department> Departments { get; set; }

    public virtual DbSet<Expense> Expenses { get; set; }

    public virtual DbSet<Prediction> Predictions { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Server=SETAH;Database=BudgetTrackDB;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Company
        modelBuilder.Entity<Company>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Companie__3214EC0714D80A49");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Name).HasMaxLength(200);
            entity.Property(e => e.PeriodType).HasMaxLength(20);
        });

        // Department
        modelBuilder.Entity<Department>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Departme__3214EC0715BD80CD");

            entity.Property(e => e.AnnualBudget).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.Name).HasMaxLength(200);
            entity.Property(e => e.Year);
            entity.Property(e => e.PeriodType).HasMaxLength(20);
            entity.HasOne(d => d.Company).WithMany(p => p.Departments)
                .HasForeignKey(d => d.CompanyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Departments_Companies");
        });

        // Expense
        modelBuilder.Entity<Expense>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Expenses__3214EC070C61AEB8");

            entity.Property(e => e.Amount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.Description).HasMaxLength(500);

            entity.HasOne(d => d.Department).WithMany(p => p.Expenses)
                .HasForeignKey(d => d.DepartmentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Expenses_Departments");
        });

        // Prediction
        modelBuilder.Entity<Prediction>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Predicti__3214EC0700D67C61");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.PredictedAmount).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.Department).WithMany(p => p.Predictions)
                .HasForeignKey(d => d.DepartmentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Predictions_Departments");
        });

        // User
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Users__3214EC07BDC417B5");

            entity.HasIndex(e => e.Email, "UQ__Users__A9D1053444967948").IsUnique();

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.Email).HasMaxLength(200);
            entity.Property(e => e.Name).HasMaxLength(200);
            entity.Property(e => e.PasswordHash).HasMaxLength(500);
            entity.Property(e => e.Role).HasMaxLength(50);

            entity.HasOne(d => d.Company).WithMany(p => p.Users)
                .HasForeignKey(d => d.CompanyId)
                .HasConstraintName("FK_Users_Companies");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}