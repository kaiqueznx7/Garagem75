using Garagem75.Shared.Dtos;
using System.Net.Http.Json;

public class DashboardApi
{
    private readonly HttpClient _http;

    public DashboardApi(HttpClient http)
    {
        _http = http;
    }

    public async Task<decimal> GetFaturamentoDia()
    {
        try
        {
            var response = await _http.GetAsync("api/dashboard/faturamento-dia");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<decimal>();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[DashboardApi Error] GetFaturamentoDia: {ex.Message}");
        }

        return 0m;
    }

    public async Task<List<ItemGrafico>> GetFabricantes()
    {
        try
        {
            var response = await _http.GetAsync("api/dashboard/fabricantes");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<ItemGrafico>>() ?? new();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[DashboardApi Error] GetFabricantes: {ex.Message}");
        }

        return new List<ItemGrafico>();
    }

    public async Task<List<ItemGrafico>> GetMarcasPecas()
    {
        try
        {
            var response = await _http.GetAsync("api/dashboard/marcas-pecas");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<ItemGrafico>>() ?? new();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[DashboardApi Error] GetMarcasPecas: {ex.Message}");
        }

        return new List<ItemGrafico>();
    }

    public async Task<DashboardDto> GetDashboard()
    {
        try
        {
            var response = await _http.GetAsync("api/dashboard");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<DashboardDto>() ?? new();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[DashboardApi Error] GetDashboard: {ex.Message}");
        }

        return new DashboardDto();
    }
}

// ── Gráficos simples ──────────────────────────────────────────────
public class ItemGrafico
{
    public string Nome { get; set; } = string.Empty;
    public int Total { get; set; }
}