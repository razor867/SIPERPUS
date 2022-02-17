using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SIPERPUS.Models;

public class SIPERPUSContext : DbContext
{
    public SIPERPUSContext(DbContextOptions<SIPERPUSContext> options) : base(options)
    {

    }
    public DbSet<Mahasiswa> Mahasiswa { get; set; }
    public DbSet<Jurusan> Jurusan { get; set; }
    public DbSet<Buku> Buku { get; set; }
    public DbSet<KategoriBuku> KategoriBuku { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<Mahasiswa>().HasKey(k => new { k.id });
        builder.Entity<Jurusan>().HasKey(k => new { k.id });
        builder.Entity<Buku>().HasKey(k => new { k.id });
        builder.Entity<KategoriBuku>().HasKey(k => new { k.id });

        #region Mahasiswa
        builder.Entity<Mahasiswa>()
            .HasOne(x => x.Jurusan)
            .WithMany(x => x.Mahasiswa)
            .HasForeignKey(x => new { x.jurusan })
            .HasPrincipalKey(x => new { x.id });
        #endregion

        #region Buku
        builder.Entity<Buku>()
            .HasOne(x => x.KategoriBuku)
            .WithMany(x => x.Buku)
            .HasForeignKey(x => new { x.kategori })
            .HasPrincipalKey(x => new { x.id });
        #endregion
    }
}