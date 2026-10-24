namespace MsProduccionAlimentos.Api.Seguridad;

public record UsuarioDemo(string Usuario, string Password, string Rol);

/// <summary>
/// Usuarios de demostración para la defensa del proyecto. En un escenario real, estas
/// credenciales vendrían de un microservicio de Identidad con contraseñas hasheadas
/// (ej. ASP.NET Core Identity + base de datos propia); aquí se simplifica a una lista en
/// memoria por alcance del proyecto académico. El mecanismo de emisión/validación de JWT
/// que sí se implementa es el mismo que se usaría con un proveedor real.
/// </summary>
public static class UsuariosDemo
{
    public static readonly List<UsuarioDemo> Lista = new()
    {
        new UsuarioDemo("admin", "Admin123!", "Administrador"),
        new UsuarioDemo("nutricionista", "Nutri123!", "Nutricionista"),
        new UsuarioDemo("operador", "Operador123!", "Operador"),
    };
}
