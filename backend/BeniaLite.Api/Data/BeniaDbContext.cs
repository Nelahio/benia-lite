using System;
using BeniaLite.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace BeniaLite.Api.Data;

public sealed class BeniaDbContext : DbContext
{
    public BeniaDbContext(DbContextOptions<BeniaDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Routine> Routines => Set<Routine>();
    public DbSet<RoutineStep> RoutineSteps => Set<RoutineStep>();
    public DbSet<RoutineCompletion> RoutineCompletions => Set<RoutineCompletion>();
    public DbSet<SymptomLog> SymptomLogs => Set<SymptomLog>();
    public DbSet<TriggerTag> TriggerTags => Set<TriggerTag>();
    public DbSet<SymptomLogTrigger> SymptomLogsTrigger => Set<SymptomLogTrigger>();
    public DbSet<Photo> Photos => Set<Photo>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(b =>
        {
            b.HasKey(x => x.Id);

            b.Property(x => x.Email)
                .IsRequired()
                .HasMaxLength(320);

            b.HasIndex(x => x.Email).IsUnique();

            b.Property(x => x.PasswordHash)
                .IsRequired();

            b.Property(x => x.CreatedAtUtc)
                .IsRequired();
        });

        modelBuilder.Entity<Routine>(b =>
        {
            b.HasKey(x => x.Id);
            b.Property(x => x.Name).IsRequired().HasMaxLength(200);
            b.Property(x => x.Category).IsRequired().HasMaxLength(50);
            b.Property(x => x.CreatedAtUtc).IsRequired();
            b.HasMany(x => x.Steps).WithOne(x => x.Routine!).HasForeignKey(x => x.RoutineId);
            b.HasIndex(x => new { x.UserId, x.Name });
        });

        modelBuilder.Entity<RoutineStep>(b =>
        {
            b.HasKey(x => x.Id);
            b.Property(x => x.Title).IsRequired().HasMaxLength(200);
            b.Property(x => x.FrequencyType).IsRequired().HasMaxLength(20);
            b.Property(x => x.FrequencyValue).IsRequired();
            b.Property(x => x.SortOrder).IsRequired();
            b.HasIndex(x => new { x.RoutineId, x.SortOrder }).IsUnique();
        });

        modelBuilder.Entity<RoutineCompletion>(b =>
        {
            b.HasKey(x => x.Id);
            b.Property(x => x.CompletedAtUtc).IsRequired();
            b.HasIndex(x => new { x.UserId, x.RoutineId, x.CompletedAtUtc });
        });

        modelBuilder.Entity<SymptomLog>(b =>
        {
            b.HasKey(x => x.Id);
            b.Property(x => x.Category).IsRequired().HasMaxLength(50);
            b.Property(x => x.Severity0to10).IsRequired();
            b.Property(x => x.LoggedAtUtc).IsRequired();

            b.HasIndex(x => new { x.UserId, x.LoggedAtUtc });
        });

        modelBuilder.Entity<TriggerTag>(b =>
        {
            b.HasKey(x => x.Id);
            b.Property(x => x.Name).IsRequired().HasMaxLength(50);
            b.HasIndex(x => new { x.UserId, x.Name }).IsUnique();
        });

        modelBuilder.Entity<SymptomLogTrigger>(b =>
        {
            b.HasKey(x => new { x.SymptomLogId, x.TriggerTagId });

            b.HasOne(x => x.SymptomLog)
                .WithMany(x => x.Triggers)
                .HasForeignKey(x => x.SymptomLogId);

            b.HasOne(x => x.TriggerTag)
                .WithMany(x => x.SymptomLogs)
                .HasForeignKey(x => x.TriggerTagId);
        });

        modelBuilder.Entity<Photo>(b =>
        {
            b.HasKey(x => x.Id);
            b.Property(x => x.Url).IsRequired().HasMaxLength(1000);
            b.Property(x => x.CreatedAtUtc).IsRequired();

            b.HasOne(x => x.SymptomLog)
                .WithMany(x => x.Photos)
                .HasForeignKey(x => x.SymptomLogId);

            b.HasIndex(x => new { x.UserId, x.CreatedAtUtc });
        });
    }
}
