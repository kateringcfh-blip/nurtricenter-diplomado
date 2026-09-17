using System.Net.Http.Json;
using MsProduccionAlimentos.Application.Interfaces;

namespace MsProduccionAlimentos.Infrastructure.ExternalClients;

public class PacientesApiClient : IPacientesApiClient
{
    private readonly HttpClient _httpClient;

    public PacientesApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<PacienteInfoDto?> ObtenerPaciente(Guid pacienteId, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync($"/api/pacientes/{pacienteId}", cancellationToken);

        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            return null;

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<PacienteInfoDto>(cancellationToken: cancellationToken);
    }
}
