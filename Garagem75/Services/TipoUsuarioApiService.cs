using Garagem75.Shared.Dtos;
using System.Net.Http.Json;

namespace Garagem75.Services
{
    public class TipoUsuarioApiService
    {
        private readonly HttpClient _http;

        public TipoUsuarioApiService(HttpClient http)
        {
            _http = http;
        }

        public async Task<List<TipoUsuarioDto>> GetAll()
        {
            var response = await _http.GetFromJsonAsync<List<TipoUsuarioDto>>("api/tipousuario");
            return response ?? new List<TipoUsuarioDto>();
        }

        public async Task<TipoUsuarioDto?> GetById(int id)
        {
            return await _http.GetFromJsonAsync<TipoUsuarioDto>($"api/tipousuario/{id}");
        }

        public async Task<bool> Create(TipoUsuarioDto dto)
        {
            var response = await _http.PostAsJsonAsync("api/tipousuario", dto);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> Update(TipoUsuarioDto dto)
        {
            var response = await _http.PutAsJsonAsync($"api/tipousuario/{dto.IdTipoUsuario}", dto);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> Delete(int id)
        {
            var response = await _http.DeleteAsync($"api/tipousuario/{id}");
            return response.IsSuccessStatusCode;
        }
    }
}