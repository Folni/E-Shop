using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace WebAPI.Models;

public partial class EtrgovinaContext : DbContext
{
    public EtrgovinaContext()
    {
    }

    public EtrgovinaContext(DbContextOptions<EtrgovinaContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Drzava> Drzavas { get; set; }

    public virtual DbSet<Kategorija> Kategorijas { get; set; }

    public virtual DbSet<Korisnik> Korisniks { get; set; }

    public virtual DbSet<Logovi> Logovis { get; set; }

    public virtual DbSet<Narudzba> Narudzbas { get; set; }

    public virtual DbSet<NarudzbaProizvod> NarudzbaProizvods { get; set; }

    public virtual DbSet<Proizvod> Proizvods { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            // "Name=DefaultConnection" govori EF-u: 
            // "Potraži u appsettings.json pod ConnectionStrings sekcijom naziv 'DefaultConnection'"
            optionsBuilder.UseSqlServer("Name=DefaultConnection");
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Drzava>(entity =>
        {
            entity.HasKey(e => e.DrzavaId).HasName("PK__Drzava__89CED846AB44AADB");

            entity.ToTable("Drzava");

            entity.HasIndex(e => e.Naziv, "UQ__Drzava__603E81462019D513").IsUnique();

            entity.Property(e => e.DrzavaId).HasColumnName("DrzavaID");
            entity.Property(e => e.Naziv).HasMaxLength(100);
        });

        modelBuilder.Entity<Kategorija>(entity =>
        {
            entity.HasKey(e => e.KategorijaId).HasName("PK__Kategori__6C3B8FCE7103C613");

            entity.ToTable("Kategorija");

            entity.HasIndex(e => e.Naziv, "UQ__Kategori__603E81462178D20E").IsUnique();

            entity.Property(e => e.KategorijaId).HasColumnName("KategorijaID");
            entity.Property(e => e.Naziv).HasMaxLength(100);
        });

        modelBuilder.Entity<Korisnik>(entity =>
        {
            entity.HasKey(e => e.KorisnikId).HasName("PK__Korisnik__80B06D61401BA6FA");

            entity.ToTable("Korisnik");

            entity.HasIndex(e => e.Email, "UQ__Korisnik__A9D105347A167286").IsUnique();

            entity.Property(e => e.KorisnikId).HasColumnName("KorisnikID");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.Guid).HasDefaultValueSql("(newid())");
            entity.Property(e => e.Ime).HasMaxLength(50);
            entity.Property(e => e.LozinkaHash).HasMaxLength(255);
            entity.Property(e => e.Prezime).HasMaxLength(50);
            entity.Property(e => e.Uloga)
                .HasMaxLength(20)
                .HasDefaultValue("Korisnik");
        });

        modelBuilder.Entity<Logovi>(entity =>
        {
            entity.HasKey(e => e.LogId).HasName("PK__Logovi__5E5499A88F835F2D");

            entity.ToTable("Logovi");

            entity.Property(e => e.LogId).HasColumnName("LogID");
            entity.Property(e => e.Datum)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.KorisnikId).HasColumnName("KorisnikID");
            entity.Property(e => e.Tip).HasMaxLength(50);

            entity.HasOne(d => d.Korisnik).WithMany(p => p.Logovis)
                .HasForeignKey(d => d.KorisnikId)
                .HasConstraintName("FK_Logovi_Korisnik");
        });

        modelBuilder.Entity<Narudzba>(entity =>
        {
            entity.HasKey(e => e.NarudzbaId).HasName("PK__Narudzba__FBEC1357E9C4E38C");

            entity.ToTable("Narudzba");

            entity.Property(e => e.NarudzbaId).HasColumnName("NarudzbaID");
            entity.Property(e => e.DatumKreiranja)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.KorisnikId).HasColumnName("KorisnikID");
            entity.Property(e => e.UkupnaCijena)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.Korisnik).WithMany(p => p.Narudzbas)
                .HasForeignKey(d => d.KorisnikId)
                .HasConstraintName("FK_Narudzba_Korisnik");
        });

        modelBuilder.Entity<NarudzbaProizvod>(entity =>
        {
            entity.HasKey(e => new { e.NarudzbaId, e.ProizvodId }).HasName("PK__Narudzba__69F698B6C4AB5DB3");

            entity.ToTable("NarudzbaProizvod");

            entity.Property(e => e.NarudzbaId).HasColumnName("NarudzbaID");
            entity.Property(e => e.ProizvodId).HasColumnName("ProizvodID");
            entity.Property(e => e.Kolicina).HasDefaultValue(1);

            entity.HasOne(d => d.Narudzba).WithMany(p => p.NarudzbaProizvods)
                .HasForeignKey(d => d.NarudzbaId)
                .HasConstraintName("FK_NarudzbaProizvod_Narudzba");

            entity.HasOne(d => d.Proizvod).WithMany(p => p.NarudzbaProizvods)
                .HasForeignKey(d => d.ProizvodId)
                .HasConstraintName("FK_NarudzbaProizvod_Proizvod");
        });

        modelBuilder.Entity<Proizvod>(entity =>
        {
            entity.HasKey(e => e.ProizvodId).HasName("PK__Proizvod__21A8BE18B781858A");

            entity.ToTable("Proizvod");

            entity.Property(e => e.ProizvodId).HasColumnName("ProizvodID");
            entity.Property(e => e.Cijena).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.KategorijaId).HasColumnName("KategorijaID");
            entity.Property(e => e.Naziv).HasMaxLength(100);
            entity.Property(e => e.SlikaPath).HasMaxLength(255);

            entity.HasOne(d => d.Kategorija).WithMany(p => p.Proizvods)
                .HasForeignKey(d => d.KategorijaId)
                .HasConstraintName("FK_Proizvod_Kategorija");

            entity.HasMany(d => d.Drzavas).WithMany(p => p.Proizvods)
                .UsingEntity<Dictionary<string, object>>(
                    "ProizvodDrzava",
                    r => r.HasOne<Drzava>().WithMany()
                        .HasForeignKey("DrzavaId")
                        .HasConstraintName("FK_ProizvodDrzava_Drzava"),
                    l => l.HasOne<Proizvod>().WithMany()
                        .HasForeignKey("ProizvodId")
                        .HasConstraintName("FK_ProizvodDrzava_Proizvod"),
                    j =>
                    {
                        j.HasKey("ProizvodId", "DrzavaId").HasName("PK__Proizvod__5934539CCAA25BA8");
                        j.ToTable("ProizvodDrzava");
                        j.IndexerProperty<int>("ProizvodId").HasColumnName("ProizvodID");
                        j.IndexerProperty<int>("DrzavaId").HasColumnName("DrzavaID");
                    });
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
