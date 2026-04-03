using Garagem75.Shared.Dtos;

public class DashboardApiService
{
    private readonly HttpClient _http;

    public DashboardApiService(HttpClient http)
    {
        _http = http;
    }

    public async Task<DashboardDto?> Get()
    {
        return await _http.GetFromJsonAsync<DashboardDto>("api/dashboard");
    }
}