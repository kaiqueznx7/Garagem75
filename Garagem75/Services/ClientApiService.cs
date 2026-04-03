using Garagem75.Shared.Dtos;

public class ClienteApiService
{
    private readonly HttpClient _http;

    public ClienteApiService(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<ClienteDto>> GetAll()
        => await _http.GetFromJsonAsync<List<ClienteDto>>("api/clientes") ?? new();

    public async Task<ClienteDto?> GetById(int id)
        => await _http.GetFromJsonAsync<ClienteDto>($"api/clientes/{id}");

    public async Task Create(ClienteDto dto)
        => await _http.PostAsJsonAsync("api/clientes", dto);

    public async Task Update(ClienteDto dto)
        => await _http.PutAsJsonAsync($"api/clientes/{dto.Id}", dto);

    public async Task Delete(int id)
        => await _http.DeleteAsync($"api/clientes/{id}");
}