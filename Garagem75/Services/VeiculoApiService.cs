using Garagem75.Shared.Dtos;
using Garagem75.Shared.Models;
using System.Net.Http.Json;

public class VeiculoApiService
{
    private readonly HttpClient _http;

    public VeiculoApiService(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<VeiculoDto>> GetAll(string? placa = null, string? cliente = null)
    {
        var url = "api/veiculo";

        if (!string.IsNullOrEmpty(placa) || !string.IsNullOrEmpty(cliente))
        {
            url += $"?searchPlaca={placa}&searchCliente={cliente}";
        }

        return await _http.GetFromJsonAsync<List<VeiculoDto>>(url) ?? new();
    }

    public async Task<VeiculoDto?> GetById(int id)
    {
        return await _http.GetFromJsonAsync<VeiculoDto>($"api/veiculo/{id}");
    }

    public async Task Create(VeiculoDto veiculo)
    {
        await _http.PostAsJsonAsync("api/veiculo", veiculo);
    }

    public async Task Update(int id, VeiculoDto veiculo)
    {
        await _http.PutAsJsonAsync($"api/veiculo/{id}", veiculo);
    }

    public async Task Delete(int id)
    {
        await _http.DeleteAsync($"api/veiculo/{id}");
    }
}