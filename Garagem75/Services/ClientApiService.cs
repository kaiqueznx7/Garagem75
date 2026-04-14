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

    public async Task<HttpResponseMessage> Create(ClienteDto cliente)
    {
        return await _http.PostAsJsonAsync("api/cliente", cliente);
        // 👆 removeu o EnsureSuccessStatusCode — agora quem trata é o Blazor
    }

    public async Task<HttpResponseMessage> Update(ClienteDto model)
    {
        // Em uma Web API RESTful, o Update geralmente usa PUT
        // Se a sua API estiver usando Post para tudo, altere para PostAsJsonAsync
        var response = await _http.PutAsJsonAsync($"api/cliente/{model.Id   }", model);

        // IMPORTANTE: Não use response.EnsureSuccessStatusCode() aqui.
        // Se você usar, o código vai travar antes de chegar no seu Controller
        // e você não conseguirá tratar o erro de "CPF já cadastrado".

        return response;
    }

    public async Task Delete(int id)
        => await _http.DeleteAsync($"api/cliente/{id}");
}