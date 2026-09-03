using MsPacientesEvaluacion.Domain.Entities;
using MsPacientesEvaluacion.Domain.Enums;
using MsPacientesEvaluacion.Domain.Events;
using MsPacientesEvaluacion.Domain.ValueObjects;

namespace MsPacientesEvaluacion.Domain.Aggregates;

public class Paciente
{
    public PacienteId Id { get; private set; }
    public string Nombre { get; private set; }
    public string Apellido { get; private set; }
    public DateOnly FechaNacimiento { get; private set; }
    public DatosContacto Contacto { get; private set; }
    public Guid NutricionistaId { get; private set; }
    public EstadoPaciente Estado { get; private set; }
    public ConsultaInicial? ConsultaInicial { get; private set; }

    private readonly List<EvaluacionSeguimiento> _evaluaciones = new();
    private readonly List<IDomainEvent> _eventos = new();

    public IReadOnlyList<EvaluacionSeguimiento> Evaluaciones => _evaluaciones.AsReadOnly();
    public IReadOnlyList<IDomainEvent> EventosOcurridos => _eventos.AsReadOnly();

    private Paciente()
    {
        Nombre = null!;
        Apellido = null!;
        Contacto = null!;
    }

    private Paciente(PacienteId id, string nombre, string apellido, DateOnly fechaNacimiento,
        DatosContacto contacto, Guid nutricionistaId)
    {
        Id = id;
        Nombre = nombre;
        Apellido = apellido;
        FechaNacimiento = fechaNacimiento;
        Contacto = contacto;
        NutricionistaId = nutricionistaId;
        Estado = EstadoPaciente.Activo;
    }

    public static Paciente Registrar(string nombre, string apellido, DateOnly fechaNacimiento,
        DatosContacto contacto, Guid nutricionistaId)
    {
        if (string.IsNullOrWhiteSpace(nombre)) throw new ArgumentException("El nombre no puede estar vacío.", nameof(nombre));
        if (string.IsNullOrWhiteSpace(apellido)) throw new ArgumentException("El apellido no puede estar vacío.", nameof(apellido));
        if (nutricionistaId == Guid.Empty) throw new ArgumentException("NutricionistaId no puede ser un Guid vacío.", nameof(nutricionistaId));
        ArgumentNullException.ThrowIfNull(contacto);

        var paciente = new Paciente(PacienteId.Crear(), nombre, apellido, fechaNacimiento, contacto, nutricionistaId);
        paciente._eventos.Add(PacienteRegistrado.Crear(paciente.Id, $"{nombre} {apellido}"));
        return paciente;
    }

    public void RegistrarConsultaInicial(DateOnly fecha, decimal peso, decimal altura, Anamnesis anamnesis)
    {
        if (ConsultaInicial is not null)
            throw new InvalidOperationException("El paciente ya tiene una consulta inicial registrada.");

        ConsultaInicial = Entities.ConsultaInicial.Registrar(fecha, peso, altura, anamnesis);
        Estado = EstadoPaciente.EnEvaluacion;
    }

    public void RegistrarEvaluacion(DateOnly fecha, decimal peso, Medidas medidas,
        string observaciones, NivelAdherencia adherencia, Guid planId)
    {
        var evaluacion = EvaluacionSeguimiento.Registrar(fecha, peso, medidas, observaciones, adherencia, planId);
        _evaluaciones.Add(evaluacion);
    }

    public void AsignarNutricionista(Guid nutricionistaId)
    {
        if (nutricionistaId == Guid.Empty)
            throw new ArgumentException("NutricionistaId no puede ser un Guid vacío.", nameof(nutricionistaId));
        NutricionistaId = nutricionistaId;
    }

    public void Desactivar()
    {
        if (Estado == EstadoPaciente.Inactivo)
            throw new InvalidOperationException("El paciente ya está inactivo.");
        Estado = EstadoPaciente.Inactivo;
    }

    public void LimpiarEventos() => _eventos.Clear();
}
