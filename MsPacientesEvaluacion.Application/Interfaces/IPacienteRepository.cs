using MsPacientesEvaluacion.Domain.Aggregates;
using MsPacientesEvaluacion.Domain.ValueObjects;

namespace MsPacientesEvaluacion.Application.Interfaces;

public interface IPacienteRepository
{
    Task Agregar(Paciente paciente);
    Task<Paciente?> ObtenerPorId(PacienteId id);
    Task<List<Paciente>> ObtenerTodos();
}
