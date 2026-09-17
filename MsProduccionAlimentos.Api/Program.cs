using MsProduccionAlimentos.Application.Commands.GenerarOrden;
using MsProduccionAlimentos.Infrastructure;
using MsProduccionAlimentos.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(GenerarOrdenCommand).Assembly));

builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Aplicar migraciones autom�ticamente al arrancar
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
app.MapControllers();

app.Run();

public partial class Program { }
