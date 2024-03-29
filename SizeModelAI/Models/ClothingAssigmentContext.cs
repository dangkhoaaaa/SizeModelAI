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
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
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
            entity.HasKey(e => e.Id).HasName("PK__Clothing__3214EC0730623992");

            entity.ToTable("Clothing");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.CollarStyle)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("Collar_style");
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
            entity.Property(e => e.SleeveLength)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("Sleeve_length");
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
                .HasConstraintName("FK__ClothingS__Cloth__38996AB5");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
