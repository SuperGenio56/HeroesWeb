using System;
using System.Collections.Generic;
using HeroesWeb.Models;
using Microsoft.EntityFrameworkCore;

namespace HeroesWeb.Data;

public partial class HeroesContext : DbContext
{
    public HeroesContext()
    {
    }

    public HeroesContext(DbContextOptions<HeroesContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Heroes> Heroes { get; set; }

    public virtual DbSet<SuperPoderes> SuperPoderes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Heroes>(entity =>
        {
            entity.HasKey(e => e.Id);
        });

        modelBuilder.Entity<SuperPoderes>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.HasOne(d => d.Heroe).WithMany(p => p.SuperPoderes)
                .HasForeignKey(d => d.HeroeId)
                .HasConstraintName("FK_SuperPoderes_Heroes");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
