using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace ejemploRecuperacion.Models;

public partial class BdRecuperarContext : DbContext
{
    public BdRecuperarContext()
    {
    }

    public BdRecuperarContext(DbContextOptions<BdRecuperarContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Usuario> Usuarios { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseNpgsql("Name=PostgresConnection");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("usuarios_pkey");

            entity.ToTable("usuarios");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Contraseña)
                .HasColumnType("character varying")
                .HasColumnName("contraseña");
            entity.Property(e => e.Email)
                .HasColumnType("character varying")
                .HasColumnName("email");
            entity.Property(e => e.FechaRegistro)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("fecha_registro");
            entity.Property(e => e.TokenRecuperacion)
                .HasColumnType("character varying")
                .HasColumnName("token_recuperacion");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
