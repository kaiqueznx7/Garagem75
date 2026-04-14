using Garagem75.Shared.Dtos;

using Microsoft.AspNetCore.Http; // Adicione esta referência
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Garagem75.Client.Services
{
    public class UsuarioApiService
    {
        private readonly HttpClient _http;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UsuarioApiService(HttpClient http, IHttpContextAccessor httpContextAccessor)
        {
            _http = http;
            _httpContextAccessor = httpContextAccessor;
        }

        //private void AdicionarToken()
        //{
        //    // Busca o token que salvamos na Claim durante o Login
        //    var token = _httpContextAccessor.HttpContext?.User.FindFirst("JWToken")?.Value;

        //    if (!string.IsNullOrEmpty(token))
        //    {
        //        _http.DefaultRequestHeaders.Authorization =
        //            new AuthenticationHeaderValue("Bearer", token);
        //    }
        //}
        // Use este método para não repetir código em todo lugar
        private void PrepararCabecalho()
        {
            var token = _httpContextAccessor.HttpContext?.User.FindFirst("JWToken")?.Value;

            if (!string.IsNullOrEmpty(token))
            {
                _http.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            }
        }

        // Certifique-se de que a classe LoginResponse tenha: Token, Nome e Tipo
        public async Task<LoginResponse?> Login(string email, string senha)
        {
            var response = await _http.PostAsJsonAsync("api/usuario/login", new
            {
                Email = email,
                Senha = senha
            });

            if (!response.IsSuccessStatusCode)
                return null;

            // Lê o objeto completo enviado pela API (token, nome, tipo)
            return await response.Content.ReadFromJsonAsync<LoginResponse>();
        }

        // ================= GET ALL =================
        public async Task<List<UsuarioDto>> GetAll()
        {
            PrepararCabecalho();
            

            // 3. Faz a chamada
            var response = await _http.GetAsync("api/usuario");

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<UsuarioDto>>() ?? new List<UsuarioDto>();
            }

            return new List<UsuarioDto>();
        }

        // ================= GET BY ID =================
        public async Task<UsuarioDto?> GetById(int id)
        {
            try
            {
                PrepararCabecalho(); // 🔥 ESSENCIAL: Sem isso a API dá 401
                return await _http.GetFromJsonAsync<UsuarioDto>($"api/usuario/{id}");
            }
            catch
            {
                return null;
            }
        }

        // ================= CREATE =================
        public async Task<HttpResponseMessage> Create(UsuarioDto usuario)
        {
            PrepararCabecalho();
            return await _http.PostAsJsonAsync("api/usuario", usuario);
        }

        // ================= UPDATE =================
        public async Task<HttpResponseMessage> Update(UsuarioDto usuario)
        {
            PrepararCabecalho();
            return await _http.PutAsJsonAsync($"api/usuario/{usuario.IdUsuario}", usuario);
        }
        // ================= DELETE =================
        public async Task<bool> Delete(int id)
        {
            PrepararCabecalho(); // 🔥 ESSENCIAL
            var response = await _http.DeleteAsync($"api/usuario/{id}");
            return response.IsSuccessStatusCode;
        }
        public async Task Reativar(int id)
        {
            PrepararCabecalho();
            await _http.PutAsync($"api/usuario/{id}/reativar", null);
        }
    }
}