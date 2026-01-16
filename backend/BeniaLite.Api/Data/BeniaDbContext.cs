using System;
using BeniaLite.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace BeniaLite.Api.Data;

public sealed class BeniaDbContext : DbContext
{
    public BeniaDbContext(DbContextOptions<BeniaDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();

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
    }
}
