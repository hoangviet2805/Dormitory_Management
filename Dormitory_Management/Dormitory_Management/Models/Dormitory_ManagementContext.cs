using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Dormitory_Management.Models;

public partial class Dormitory_ManagementContext : DbContext
{
    public Dormitory_ManagementContext()
    {
    }

    public Dormitory_ManagementContext(DbContextOptions<Dormitory_ManagementContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Fee> Fees { get; set; }

    public virtual DbSet<Room> Rooms { get; set; }

    public virtual DbSet<Student> Students { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer(config.GetConnectionString("DB"));
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Fee>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("fees");

            entity.Property(e => e.Amount).HasColumnName("amount");
            entity.Property(e => e.Fmonth)
                .HasMaxLength(60)
                .IsUnicode(false)
                .HasColumnName("fmonth");
            entity.Property(e => e.MobileNo).HasColumnName("mobileNo");

            entity.HasOne(d => d.MobileNoNavigation).WithMany()
                .HasPrincipalKey(p => p.Mobile)
                .HasForeignKey(d => d.MobileNo)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__fees__mobileNo__52593CB8");
        });

        modelBuilder.Entity<Room>(entity =>
        {
            entity.HasKey(e => e.RoomNo).HasName("PK__rooms__6C3BFE6DF28C580A");

            entity.ToTable("rooms");

            entity.Property(e => e.RoomNo)
                .ValueGeneratedNever()
                .HasColumnName("roomNo");
            entity.Property(e => e.Booked)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasDefaultValue("No");
            entity.Property(e => e.RoomStatus)
                .HasMaxLength(250)
                .IsUnicode(false)
                .HasColumnName("roomStatus");
        });

        modelBuilder.Entity<Student>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Student__3213E83F00530F93");

            entity.ToTable("Student");

            entity.HasIndex(e => e.Mobile, "idx_mobile").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Email)
                .HasMaxLength(250)
                .IsUnicode(false)
                .HasColumnName("email");
            entity.Property(e => e.Fname)
                .HasMaxLength(250)
                .IsUnicode(false)
                .HasColumnName("fname");
            entity.Property(e => e.Idproof)
                .HasMaxLength(250)
                .IsUnicode(false)
                .HasColumnName("idproof");
            entity.Property(e => e.Living)
                .HasMaxLength(250)
                .IsUnicode(false)
                .HasDefaultValue("Yes")
                .HasColumnName("living");
            entity.Property(e => e.Mname)
                .HasMaxLength(250)
                .IsUnicode(false)
                .HasColumnName("mname");
            entity.Property(e => e.Mobile).HasColumnName("mobile");
            entity.Property(e => e.Name)
                .HasMaxLength(250)
                .IsUnicode(false)
                .HasColumnName("name");
            entity.Property(e => e.Paddress)
                .HasMaxLength(250)
                .IsUnicode(false)
                .HasColumnName("paddress");
            entity.Property(e => e.RoomNo).HasColumnName("roomNo");

            entity.HasOne(d => d.RoomNoNavigation).WithMany(p => p.Students)
                .HasForeignKey(d => d.RoomNo)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Student__roomNo__5070F446");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__users__1788CC4C428186D7");

            entity.ToTable("users");

            entity.HasIndex(e => e.Username, "UQ__users__536C85E4BA37FD8B").IsUnique();

            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.Username).HasMaxLength(50);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
