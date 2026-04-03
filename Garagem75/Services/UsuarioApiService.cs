using System.Net.Http.Json;
using Garagem75.Shared.Dtos;

namespace Garagem75.Client.Services
{
    public class UsuarioApiService
    {
        private readonly HttpClient _http;

        public UsuarioApiService(HttpClient http)
        {
            _http = http;
        }

        // ================= LOGIN =================
        public async Task<LoginResponseDto?> Login(string email, string senha)
        {
            var response = await _http.PostAsJsonAsync("api/usuario/login", new
            {
                email,
                senha
            });

            if (!response.IsSuccessStatusCode)
                return null;

            return await response.Content.ReadFromJsonAsync<LoginResponseDto>();
        }

        // ================= GET ALL =================
        public async Task<List<UsuarioDto>> GetAll()
        {
            var result = await _http.GetFromJsonAsync<List<UsuarioDto>>("api/usuario");
            return result ?? new List<UsuarioDto>();
        }

        // ================= GET BY ID =================
        public async Task<UsuarioDto?> GetById(int id)
        {
            try
            {
                return await _http.GetFromJsonAsync<UsuarioDto>($"api/usuario/{id}");
            }
            catch
            {
                return null;
            }
        }

        // ================= CREATE =================
        public async Task<bool> Create(UsuarioDto usuario)
        {
            var response = await _http.PostAsJsonAsync("api/usuario", usuario);
            return response.IsSuccessStatusCode;
        }

        // ================= UPDATE =================
        public async Task<bool> Update(UsuarioDto usuario)
        {
            var response = await _http.PutAsJsonAsync($"api/usuario/{usuario.IdUsuario}", usuario);
            return response.IsSuccessStatusCode;
        }

        // ================= DELETE =================
        public async Task<bool> Delete(int id)
        {
            var response = await _http.DeleteAsync($"api/usuario/{id}");
            return response.IsSuccessStatusCode;
        }
    }
}