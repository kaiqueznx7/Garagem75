using System.Net.Http;
using System.Net.Http.Json;
using Garagem75.Shared.Dtos;

public class OrdemServicoApiService
{
    private readonly HttpClient _http;

    public OrdemServicoApiService(HttpClient http)
    {
        _http = http;
    }



    // 🔹 GET ALL
    public async Task<List<OrdemServicoDto>> GetAll(string? search = null)
    {
        var url = "api/ordemservico";

        if (!string.IsNullOrEmpty(search))
            url += $"?search={search}";

        var result = await _http.GetFromJsonAsync<List<OrdemServicoDto>>(url);
        return result ?? new List<OrdemServicoDto>();
    }

    // 🔹 GET BY ID
    public async Task<OrdemServicoDto?> GetById(int id)
    {
        return await _http.GetFromJsonAsync<OrdemServicoDto>($"api/ordemservico/{id}");
    }

    // 🔹 CREATE
    public async Task<bool> Create(OrdemServicoDto dto)
    {
        var response = await _http.PostAsJsonAsync("api/ordemservico", dto);
        return response.IsSuccessStatusCode;
    }

    // 🔹 UPDATE
    public async Task<bool> Update(int id, OrdemServicoDto dto)
    {
        var response = await _http.PutAsJsonAsync($"api/ordemservico/{id}", dto);
        return response.IsSuccessStatusCode;
    }

    // 🔹 DELETE
    public async Task<bool> Delete(int id)
    {
        var response = await _http.DeleteAsync($"api/ordemservico/{id}");
        return response.IsSuccessStatusCode;
    }
}