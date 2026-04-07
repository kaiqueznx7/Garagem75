using Garagem75.Shared.Dtos;
using System.Net.Http.Json;

namespace Garagem75.Client.Services
{
    public class EnderecoApiService
    {
        private readonly HttpClient _http;

        public EnderecoApiService(HttpClient http)
        {
            _http = http;
        }

        // Listar todos os endereços
        public async Task<List<EnderecoDto>> GetAll()
        {
            try
            {
                var response = await _http.GetFromJsonAsync<List<EnderecoDto>>("api/endereco");
                return response ?? new List<EnderecoDto>();
            }
            catch
            {
                return new List<EnderecoDto>();
            }
        }

        // Buscar um endereço por ID
        public async Task<EnderecoDto?> GetById(int id)
        {
            return await _http.GetFromJsonAsync<EnderecoDto>($"api/endereco/{id}");
        }

        // Criar novo endereço
        public async Task<bool> Create(EnderecoDto dto)
        {
            var response = await _http.PostAsJsonAsync("api/endereco", dto);
            return response.IsSuccessStatusCode;
        }

        // Atualizar endereço existente
        public async Task<bool> Update(EnderecoDto dto)
        {
            var response = await _http.PutAsJsonAsync($"api/endereco/{dto.IdEndereco}", dto);
            return response.IsSuccessStatusCode;
        }

        // Deletar endereço
        public async Task<bool> Delete(int id)
        {
            var response = await _http.DeleteAsync($"api/endereco/{id}");
            return response.IsSuccessStatusCode;
        }
    }
}