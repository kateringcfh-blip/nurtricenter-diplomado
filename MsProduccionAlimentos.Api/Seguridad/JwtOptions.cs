namespace MsProduccionAlimentos.Api.Seguridad;

/// <summary>
/// Configuración de firma/validación de JWT, leída desde la sección "Jwt" de appsettings.
/// La misma clave, issuer y audience se usan en MsPacientesEvaluacion.Api, de modo que un
/// token emitido por cualquiera de los dos microservicios es válido en ambos (simula un
/// Identity Provider centralizado sin necesitar desplegar un tercer microservicio).
/// </summary>
public class JwtOptions
{
    public string Key { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public int ExpiracionMinutos { get; set; } = 60;
}
