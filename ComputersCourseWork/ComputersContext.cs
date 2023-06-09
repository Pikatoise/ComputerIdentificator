using System;
using System.Collections.Generic;
using System.Configuration;
using Microsoft.EntityFrameworkCore;

namespace ComputersCourseWork;

// alter table computer add column inventoryNum varchar(100);

public partial class ComputersContext : DbContext
{
    public ComputersContext(string conn)
    {
        connectionString = conn;
    }

    public ComputersContext(DbContextOptions<ComputersContext> options, string conn)
        : base(options)
    {
        connectionString = conn;
    }

    private string connectionString;

    public virtual DbSet<Computer> Computers { get; set; }

    public virtual DbSet<Computerspec> Computerspecs { get; set; }

    public virtual DbSet<Devicetype> Devicetypes { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseMySql(connectionString, ServerVersion.Parse("8.0.31-mysql"));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_0900_ai_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Computer>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("computer");

            entity.HasIndex(e => e.Name, "Name").IsUnique();

            entity.HasIndex(e => e.Devicetype, "computer_devicetype_fk");

            entity.Property(e => e.Id).HasColumnName("id"); 
            entity.Property(e => e.Description).HasMaxLength(200);
            entity.Property(e => e.inventoryNum).HasMaxLength(100);
            entity.Property(e => e.Devicetype).HasColumnName("devicetype");
            entity.Property(e => e.LastUpdate).HasColumnName("lastUpdate");
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.Windowskey)
                .HasMaxLength(50)
                .HasColumnName("windowskey");

            entity.HasOne(d => d.DevicetypeNavigation).WithMany(p => p.Computers) 
                .HasForeignKey(d => d.Devicetype)
                .HasConstraintName("computer_devicetype_fk");
        });

        modelBuilder.Entity<Computerspec>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("computerspec");

            entity.HasIndex(e => e.Computer, "computerspec_computer_fk");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Computer).HasColumnName("computer");
            entity.Property(e => e.Title).HasMaxLength(50);
            entity.Property(e => e.Value).HasMaxLength(100);
            entity.Property(e => e.IsNetwork).HasDefaultValue(false);

            entity.HasOne(d => d.ComputerNavigation).WithMany(p => p.Computerspecs)
                .HasForeignKey(d => d.Computer)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("computerspec_computer_fk");
        });

        modelBuilder.Entity<Devicetype>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("devicetype");

            entity.HasIndex(e => e.Name, "Name").IsUnique();

            entity.Property(e => e.Name).HasMaxLength(20);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
