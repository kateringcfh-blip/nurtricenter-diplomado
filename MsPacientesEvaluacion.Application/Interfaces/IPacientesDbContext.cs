using MsPacientesEvaluacion.Domain.Aggregates;

namespace MsPacientesEvaluacion.Application.Interfaces;

public interface IPacientesDbContext
{
    IQueryable<Paciente> Pacientes { get; }
}
