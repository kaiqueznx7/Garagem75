using Garagem75.Shared.Dtos;

public class ClienteApiService
{
    private readonly HttpClient _http;

    public ClienteApiService(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<ClienteDto>> GetAll()
        => await _http.GetFromJsonAsync<List<ClienteDto>>("api/cliente") ?? new();

    public async Task<ClienteDto?> GetById(int id)
        => await _http.GetFromJsonAsync<ClienteDto>($"api/cliente/{id}");

    public async Task Create(ClienteDto dto)
        => await _http.PostAsJsonAsync("api/cliente", dto);

    public async Task Update(ClienteDto dto)
        => await _http.PutAsJsonAsync($"api/cliente/{dto.Id}", dto);

    public async Task Delete(int id)
        => await _http.DeleteAsync($"api/cliente/{id}");
}