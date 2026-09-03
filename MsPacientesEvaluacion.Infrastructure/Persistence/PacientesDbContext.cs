using Microsoft.EntityFrameworkCore;
using MsPacientesEvaluacion.Application.Interfaces;
using MsPacientesEvaluacion.Domain.Aggregates;
using MsPacientesEvaluacion.Domain.Enums;
using MsPacientesEvaluacion.Domain.ValueObjects;

namespace MsPacientesEvaluacion.Infrastructure.Persistence;

public class PacientesDbContext : DbContext, IPacientesDbContext
{
    public PacientesDbContext(DbContextOptions<PacientesDbContext> options) : base(options) { }

    public DbSet<Paciente> Pacientes => Set<Paciente>();

    IQueryable<Paciente> IPacientesDbContext.Pacientes => Pacientes;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ConfigurarPaciente(modelBuilder);
    }

    private static void ConfigurarPaciente(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Paciente>(entity =>
        {
            entity.ToTable("Pacientes");

            entity.HasKey(p => p.Id);
            entity.Property(p => p.Id)
                .HasConversion(id => id.Valor, guid => PacienteId.De(guid));

            entity.Property(p => p.Nombre).IsRequired().HasMaxLength(100);
            entity.Property(p => p.Apellido).IsRequired().HasMaxLength(100);
            entity.Property(p => p.FechaNacimiento).IsRequired();
            entity.Property(p => p.NutricionistaId).IsRequired();

            entity.Property(p => p.Estado)
                .HasConversion(e => e.ToString(), s => Enum.Parse<EstadoPaciente>(s))
                .HasMaxLength(20);

            entity.OwnsOne(p => p.Contacto, contacto =>
            {
                contacto.Property(c => c.Email).HasColumnName("Contacto_Email").HasMaxLength(200);
                contacto.Property(c => c.Direccion).HasColumnName("Contacto_Direccion").HasMaxLength(300);
                contacto.Property(c => c.Telefono).HasColumnName("Contacto_Telefono").HasMaxLength(20);
            });

            entity.OwnsOne(p => p.ConsultaInicial, consulta =>
            {
                consulta.Property(c => c.Id)
                    .HasConversion(id => id.Valor, guid => ConsultaId.De(guid))
                    .HasColumnName("ConsultaInicial_Id");

                consulta.Property(c => c.Fecha).HasColumnName("ConsultaInicial_Fecha");
                consulta.Property(c => c.Peso)
                    .HasColumnName("ConsultaInicial_Peso")
                    .HasPrecision(8, 2);
                consulta.Property(c => c.Altura)
                    .HasColumnName("ConsultaInicial_Altura")
                    .HasPrecision(5, 2);

                consulta.OwnsOne(c => c.Anamnesis, anamnesis =>
                {
                    anamnesis.Property(a => a.Id)
                        .HasConversion(id => id.Valor, guid => AnamnesisId.De(guid))
                        .HasColumnName("Anamnesis_Id");

                    anamnesis.Property(a => a.HabitosAlimenticios)
                        .HasColumnName("Anamnesis_HabitosAlimenticios")
                        .HasMaxLength(1000);
                    anamnesis.Property(a => a.AntecedentesClinicos)
                        .HasColumnName("Anamnesis_AntecedentesClinicos")
                        .HasMaxLength(1000);
                    anamnesis.Property(a => a.NecesidadesEspecificas)
                        .HasColumnName("Anamnesis_NecesidadesEspecificas")
                        .HasMaxLength(500);
                });
            });

            entity.OwnsMany(p => p.Evaluaciones, evaluacion =>
            {
                evaluacion.ToTable("EvaluacionesSeguimiento");

                evaluacion.WithOwner().HasForeignKey("PacienteId");

                evaluacion.HasKey(e => e.Id);
                evaluacion.Property(e => e.Id)
                    .HasConversion(id => id.Valor, guid => EvaluacionId.De(guid));

                evaluacion.Property(e => e.Fecha).IsRequired();
                evaluacion.Property(e => e.Peso).HasPrecision(8, 2);
                evaluacion.Property(e => e.Observaciones).HasMaxLength(1000);
                evaluacion.Property(e => e.PlanId).IsRequired();

                evaluacion.Property(e => e.Adherencia)
                    .HasConversion(a => a.ToString(), s => Enum.Parse<Domain.Enums.NivelAdherencia>(s))
                    .HasMaxLength(10);

                evaluacion.OwnsOne(e => e.Medidas, medidas =>
                {
                    medidas.Property(m => m.Cintura)
                        .HasColumnName("Medidas_Cintura")
                        .HasPrecision(8, 2);
                    medidas.Property(m => m.Cadera)
                        .HasColumnName("Medidas_Cadera")
                        .HasPrecision(8, 2);
                    medidas.Property(m => m.Imc)
                        .HasColumnName("Medidas_Imc")
                        .HasPrecision(6, 2);
                });
            });

            // Backing field para colección privada _evaluaciones
            entity.Navigation(p => p.Evaluaciones)
                .HasField("_evaluaciones")
                .UsePropertyAccessMode(PropertyAccessMode.Field);
        });
    }
}
