using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MsProduccionAlimentos.Api.Seguridad;

namespace MsProduccionAlimentos.Api.Controllers;

/// <summary>
/// Emite tokens JWT para autenticarse frente a cualquiera de los microservicios de
/// NurTricenter (ambos validan con la misma clave/issuer/audience). Usa usuarios de
/// demostración en memoria; ver <see cref="UsuariosDemo"/> para el detalle de esa
/// simplificación.
/// </summary>
[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly JwtOptions _jwtOptions;

    public AuthController(IOptions<JwtOptions> jwtOptions)
    {
        _jwtOptions = jwtOptions.Value;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        var usuario = UsuariosDemo.Lista.FirstOrDefault(u =>
            u.Usuario.Equals(request.Usuario, StringComparison.OrdinalIgnoreCase) &&
            u.Password == request.Password);

        if (usuario is null)
            return Unauthorized(new { error = "Usuario o contraseña inválidos." });

        var token = GenerarToken(usuario);
        return Ok(new
        {
            token,
            expiraEnMinutos = _jwtOptions.ExpiracionMinutos,
            usuario = usuario.Usuario,
            rol = usuario.Rol,
        });
    }

    private string GenerarToken(UsuarioDemo usuario)
    {
        var clave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Key));
        var credenciales = new SigningCredentials(clave, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, usuario.Usuario),
            new(ClaimTypes.Name, usuario.Usuario),
            new(ClaimTypes.Role, usuario.Rol),
        };

        var token = new JwtSecurityToken(
            issuer: _jwtOptions.Issuer,
            audience: _jwtOptions.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtOptions.ExpiracionMinutos),
            signingCredentials: credenciales);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

public record LoginRequest(string Usuario, string Password);
