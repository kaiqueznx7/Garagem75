using Garagem75.Shared.Dtos;
using Garagem75.Shared.Models;
using Microsoft.AspNetCore.Components.Forms;
using System.Net.Http.Json;

public class VeiculoApi
{
    private readonly HttpClient _http;

    public VeiculoApi(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<VeiculoDto>> GetAll()
    {
        try
        {
            var response = await _http.GetAsync("api/veiculo");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<VeiculoDto>>() ?? new();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[VeiculoApi Error] GetAll: {ex.Message}");
        }

        return new List<VeiculoDto>();
    }

    public async Task<VeiculoDto?> GetById(int id)
    {
        try
        {
            var response = await _http.GetAsync($"api/veiculo/{id}");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<VeiculoDto>();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[VeiculoApi Error] GetById: {ex.Message}");
        }

        return null;
    }

    public async Task<HttpResponseMessage> Create(VeiculoDto v)
        => await _http.PostAsJsonAsync("api/veiculo", v);

    public async Task<HttpResponseMessage> Update(int id, VeiculoDto v)
        => await _http.PutAsJsonAsync($"api/veiculo/{id}", v);

    public async Task Delete(int id)
        => await _http.DeleteAsync($"api/veiculo/{id}");

    public async Task<string?> UploadFoto(int id, IBrowserFile file)
    {
        try
        {
            var content = new MultipartFormDataContent();
            long maxFileSize = 1024 * 1024 * 15; // 15MB

            var buffer = new byte[file.Size];
            await file.OpenReadStream(maxFileSize).ReadAsync(buffer);

            var fileContent = new ByteArrayContent(buffer);
            fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(file.ContentType);

            content.Add(fileContent, "file", file.Name);

            var response = await _http.PostAsync($"api/veiculo/{id}/upload", content);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Erro no upload: {response.StatusCode} - {errorContent}");
                return null;
            }

            var result = await response.Content.ReadFromJsonAsync<UploadResult>();
            return result?.fotoUrl;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exceção no upload: {ex.Message}");
            return null;
        }
    }

    public async Task<List<VeiculoDto>> GetByCliente(int clienteId)
    {
        try
        {
            var response = await _http.GetAsync($"api/veiculo/cliente/{clienteId}");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<VeiculoDto>>() ?? new();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[VeiculoApi Error] GetByCliente: {ex.Message}");
        }

        return new List<VeiculoDto>();
    }

    public class UploadResult
    {
        public string? fotoUrl { get; set; }
    }
}