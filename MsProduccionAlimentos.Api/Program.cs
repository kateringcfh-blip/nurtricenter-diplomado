using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MsProduccionAlimentos.Api.Seguridad;
using MsProduccionAlimentos.Api.ServiceDiscovery;
using MsProduccionAlimentos.Application.Commands.GenerarOrden;
using MsProduccionAlimentos.Infrastructure;
using MsProduccionAlimentos.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(GenerarOrdenCommand).Assembly));

builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// --- Seguridad: JWT ---------------------------------------------------
// La misma clave/issuer/audience se configuran en MsPacientesEvaluacion.Api, de modo que
// un token emitido por /api/auth/login de cualquiera de los dos microservicios es válido
// en ambos (simula un Identity Provider centralizado).
builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("Jwt"));

var jwtSection = builder.Configuration.GetSection("Jwt");
var jwtKey = jwtSection["Key"]
    ?? throw new InvalidOperationException("Falta configurar la sección Jwt:Key en appsettings.");

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSection["Issuer"],
            ValidAudience = jwtSection["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ClockSkew = TimeSpan.FromSeconds(30),
        };
    });

builder.Services.AddAuthorization();

// --- Service Discovery: Consul -----------------------------------------
// Esta instancia se anuncia sola en Consul al arrancar (nombre, dirección, puerto) y se
// da de baja al apagarse. Consul además le hace health check a GET /health cada 10s.
builder.Services.RegisterServiceToServiceDiscovery(builder.Configuration);

builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Autenticación JWT. Pega: Bearer {token obtenido en /api/auth/login}",
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" },
            },
            Array.Empty<string>()
        },
    });
});

var app = builder.Build();

// Aplicar migraciones automáticamente al arrancar
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<NurTricenterDbContext>();
    db.Database.Migrate();
}

app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async ctx =>
    {
        var ex = ctx.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>()?.Error;
        ctx.Response.StatusCode = ex switch
        {
            KeyNotFoundException => 404,
            InvalidOperationException => 400,
            ArgumentException => 400,
            _ => 500
        };
        ctx.Response.ContentType = "application/json";
        await ctx.Response.WriteAsJsonAsync(new { error = ex?.Message });
    });
});

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

// Lo consulta Consul (Service Discovery) para saber si esta instancia sigue viva.
// Queda público a propósito: Consul le pega directo, sin token.
app.MapGet("/health", () => Results.Ok(new { estado = "Healthy" }));

app.MapControllers();

app.Run();

public partial class Program { }
