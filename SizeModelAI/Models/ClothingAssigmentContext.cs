using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace SizeModelAI.Models;

public partial class ClothingAssigmentContext : DbContext
{
    public ClothingAssigmentContext()
    {
    }

    public ClothingAssigmentContext(DbContextOptions<ClothingAssigmentContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Clothing> Clothings { get; set; }

    public virtual DbSet<ClothingSize> ClothingSizes { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer(GetConnectionString());


    private string GetConnectionString()
    {
        IConfiguration config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", true, true)
            .Build();
        var strConn = config["ConnectionStrings:DefaultConnection"];
        return strConn;
    }


protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Clothing>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Clothing__3214EC07E69C633B");

            entity.ToTable("Clothing");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Color)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.FabricMaterial)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Fit)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Image).IsUnicode(false);
            entity.Property(e => e.Style)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Type)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        modelBuilder.Entity<ClothingSize>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("ClothingSize");

            entity.Property(e => e.Size)
                .HasMaxLength(20)
                .IsUnicode(false);

            entity.HasOne(d => d.Clothing).WithMany()
                .HasForeignKey(d => d.ClothingId)
                .HasConstraintName("FK__ClothingS__Cloth__4AB81AF0");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
