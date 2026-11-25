using GestionProyectos.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GestionProyectos.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Proyecto> Proyectos => Set<Proyecto>();
    public DbSet<Tarea> Tareas => Set<Tarea>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Proyecto>(entity =>
        {
            entity.ToTable("proyectos");
            entity.HasKey(p => p.Id);

            entity.Property(p => p.Nombre)
                .IsRequired()
                .HasMaxLength(150);

            entity.Property(p => p.Descripcion)
                .HasMaxLength(1000);

            entity.Property(p => p.FechaInicio).IsRequired();
            entity.Property(p => p.FechaFinalizacion).IsRequired();
        });

        modelBuilder.Entity<Tarea>(entity =>
        {
            entity.ToTable("tareas");
            entity.HasKey(t => t.Id);

            entity.Property(t => t.Titulo)
                .IsRequired()
                .HasMaxLength(150);

            entity.Property(t => t.Descripcion)
                .HasMaxLength(1000);

            entity.Property(t => t.Estado)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(20);

            entity.Property(t => t.Prioridad)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(20);

            entity.Property(t => t.FechaLimite).IsRequired();

            entity.HasOne(t => t.Proyecto)
                .WithMany(p => p.Tareas)
                .HasForeignKey(t => t.ProyectoId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}