using Garagem75.Shared.Dtos;

public class PecaApiService
{
    private readonly HttpClient _http;

    public PecaApiService(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<PecaDto>> GetAll()
        => await _http.GetFromJsonAsync<List<PecaDto>>("api/peca") ?? new();

    public async Task<PecaDto?> GetById(int id)
        => await _http.GetFromJsonAsync<PecaDto>($"api/peca/{id}");

    public async Task<PecaDto> Create(PecaDto dto)
    {
        var response = await _http.PostAsJsonAsync("api/peca", dto);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<PecaDto>();
    }

    public async Task Update(PecaDto dto)
        => await _http.PutAsJsonAsync($"api/peca/{dto.IdPeca}", dto);

    public async Task Delete(int id)
        => await _http.DeleteAsync($"api/peca/{id}");

    public async Task UploadImagem(int id, IFormFile file)
    {
        using var content = new MultipartFormDataContent();

        var fileContent = new StreamContent(file.OpenReadStream());
        fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(file.ContentType);

        content.Add(fileContent, "file", file.FileName);

        var response = await _http.PostAsync($"api/peca/{id}/upload", content);

        response.EnsureSuccessStatusCode();
    }
}