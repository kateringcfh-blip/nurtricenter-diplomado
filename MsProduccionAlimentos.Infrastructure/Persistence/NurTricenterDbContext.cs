using Microsoft.EntityFrameworkCore;
using MsProduccionAlimentos.Domain.Aggregates;
using MsProduccionAlimentos.Domain.Enums;
using MsProduccionAlimentos.Domain.ValueObjects;

namespace MsProduccionAlimentos.Infrastructure.Persistence;

public class NurTricenterDbContext : DbContext
{
    public NurTricenterDbContext(DbContextOptions<NurTricenterDbContext> options) : base(options) { }

    public DbSet<OrdenProduccion> OrdenesProd => Set<OrdenProduccion>();
    public DbSet<Paquete> Paquetes => Set<Paquete>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ConfigurarOrdenProduccion(modelBuilder);
        ConfigurarPaquete(modelBuilder);
    }

    private static void ConfigurarOrdenProduccion(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<OrdenProduccion>(entity =>
        {
            entity.ToTable("OrdenesProd");

            entity.HasKey(o => o.Id);
            entity.Property(o => o.Id)
                .HasConversion(id => id.Valor, guid => OrdenId.De(guid));

            entity.Property(o => o.EncargadoCocinaId)
                .IsRequired();

            entity.Property(o => o.LoteProduccion)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(o => o.Estado)
                .HasConversion<string>()
                .HasMaxLength(30)
                .IsRequired();

            entity.Property(o => o.Fecha)
                .IsRequired();

            entity.OwnsMany(o => o.Items, item =>
            {
                item.ToTable("OrdenItems");

                item.HasKey(i => i.Id);
                item.Property(i => i.Id)
                    .HasConversion(id => id.Valor, guid => ItemOrdenId.De(guid));

                item.Property(i => i.RecetaId).IsRequired();
                item.Property(i => i.CantidadRequerida).IsRequired();
                item.Property(i => i.CantidadPreparada).IsRequired();

                item.WithOwner().HasForeignKey("OrdenProduccionId");
            });

            entity.Navigation(o => o.Items)
                .HasField("_items")
                .UsePropertyAccessMode(PropertyAccessMode.Field);
        });
    }

    private static void ConfigurarPaquete(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Paquete>(entity =>
        {
            entity.ToTable("Paquetes");

            entity.HasKey(p => p.Id);
            entity.Property(p => p.Id)
                .HasConversion(id => id.Valor, guid => PaqueteId.De(guid));

            entity.Property(p => p.PacienteId).IsRequired();

            entity.Property(p => p.OrdenId)
                .HasConversion(id => id.Valor, guid => OrdenId.De(guid))
                .IsRequired();

            entity.Property(p => p.Fecha).IsRequired();

            entity.Property(p => p.Estado)
                .HasConversion<string>()
                .HasMaxLength(30)
                .IsRequired();

            entity.OwnsOne(p => p.Etiqueta, etiqueta =>
            {
                etiqueta.Property(e => e.NombrePaciente)
                    .HasColumnName("Etiqueta_NombrePaciente")
                    .HasMaxLength(200)
                    .IsRequired();
                etiqueta.Property(e => e.DireccionEntrega)
                    .HasColumnName("Etiqueta_DireccionEntrega")
                    .HasMaxLength(300)
                    .IsRequired();
                etiqueta.Property(e => e.NumeroId)
                    .HasColumnName("Etiqueta_NumeroId")
                    .HasMaxLength(50)
                    .IsRequired();
            });

            entity.OwnsMany(p => p.Porciones, porcion =>
            {
                porcion.ToTable("Porciones");

                porcion.HasKey(p => p.Id);
                porcion.Property(p => p.Id)
                    .HasConversion(id => id.Valor, guid => PorcionId.De(guid));

                porcion.Property(p => p.RecetaId).IsRequired();
                porcion.Property(p => p.Cantidad).HasPrecision(10, 2).IsRequired();
                porcion.Property(p => p.EstaEnvasada).IsRequired();

                porcion.WithOwner().HasForeignKey("PaqueteId");
            });

            entity.Navigation(p => p.Porciones)
                .HasField("_porciones")
                .UsePropertyAccessMode(PropertyAccessMode.Field);
        });
    }
}
