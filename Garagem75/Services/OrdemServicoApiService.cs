using Garagem75.Shared.Dtos;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;

public class OrdemServicoApiService
{
    private readonly HttpClient _http;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public OrdemServicoApiService(HttpClient http, IHttpContextAccessor httpContextAccessor)
    {
        _http = http;
        _httpContextAccessor = httpContextAccessor;
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
        return await _http.GetFromJsonAsync<OrdemServicoDto>($"api/OrdemServico/{id}");
    }

    // 🔹 CREATE
    public async Task<bool> Create(OrdemServicoDto dto)
    {
        var token = _httpContextAccessor.HttpContext.Request.Cookies["jwt"];
        _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var response = await _http.PostAsJsonAsync("api/OrdemServico", dto);
        if (!response.IsSuccessStatusCode)
        {
            // Se der erro, isso vai te mostrar EXATAMENTE o que a API não gostou
            var errorContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Erro na API: {response.StatusCode}");
            Console.WriteLine($"Detalhes: {errorContent}");
            return false;
        }

        return true;
    }

    // 🔹 UPDATE
    public async Task<bool> Update(int id, OrdemServicoDto dto)
    {
        var response = await _http.PutAsJsonAsync($"api/OrdemServico/{id}", dto);
        return response.IsSuccessStatusCode;
    }

    // 🔹 DELETE
    public async Task<bool> Delete(int id)
    {
        var response = await _http.DeleteAsync($"api/OrdemServico/{id}");
        return response.IsSuccessStatusCode;
    }
}