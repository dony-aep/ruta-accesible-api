using Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Lugar> Lugares { get; set; } = null!;
    public DbSet<TipoBarrera> TiposBarrera { get; set; } = null!;
    public DbSet<ReporteAccesibilidad> Reportes { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Un lugar acumula muchos reportes. Al eliminar el lugar se eliminan sus reportes.
        modelBuilder.Entity<ReporteAccesibilidad>()
            .HasOne(r => r.Lugar)
            .WithMany(l => l.Reportes)
            .HasForeignKey(r => r.LugarId)
            .OnDelete(DeleteBehavior.Cascade);

        // Un criterio de la norma clasifica muchos reportes. No se puede eliminar un
        // criterio que ya este en uso: PLAN-03 devuelve 400 en ese caso.
        modelBuilder.Entity<ReporteAccesibilidad>()
            .HasOne(r => r.TipoBarrera)
            .WithMany(t => t.Reportes)
            .HasForeignKey(r => r.TipoBarreraId)
            .OnDelete(DeleteBehavior.Restrict);

        // El codigo del criterio es su identificador de negocio.
        modelBuilder.Entity<TipoBarrera>()
            .HasIndex(t => t.Codigo)
            .IsUnique();

        // Guardar los enum como texto y no como numero: la base de datos y la demo
        // se leen mejor, y un cambio de orden en el enum no corrompe los datos.
        modelBuilder.Entity<ReporteAccesibilidad>()
            .Property(r => r.Estado)
            .HasConversion<string>()
            .HasMaxLength(20);

        modelBuilder.Entity<ReporteAccesibilidad>()
            .Property(r => r.Severidad)
            .HasConversion<string>()
            .HasMaxLength(10);
    }
}
