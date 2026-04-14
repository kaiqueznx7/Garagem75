using Garagem75.Client.Services;
using Garagem75.Services;
using Garagem75.Shared.Dtos;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

[Authorize]

public class UsuarioController : Controller
    {
        private readonly UsuarioApiService _api;
        private readonly TipoUsuarioApiService _tipoApi; 

        public UsuarioController(UsuarioApiService api, TipoUsuarioApiService tipoApi)
        {
            _api = api;
            _tipoApi = tipoApi;
        }

        // ================= LOGIN =================

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string? returnUrl = null)
        {
            if (User?.Identity?.IsAuthenticated == true)
                return RedirectToAction("Index", "Dashboard");

            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string email, string senha)
        {
            // 1. Chama o Service
            var response = await _api.Login(email, senha);

            if (response == null || string.IsNullOrEmpty(response.token))
            {
                ModelState.AddModelError("", "Email ou senha inválidos.");
                return View();
            }

            // 2. Cria as Claims de autenticação do MVC
            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, response.nome ?? ""),
        // 🔥 AQUI ESTÁ O AJUSTE: Garanta que o valor não seja nulo ou vazio
        new Claim(ClaimTypes.Role, response.tipo ?? "Usuario"),
        // CRUCIAL: Salva o token da API no Cookie do MVC
        new Claim("JWToken", response.token)
    };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
            // Debug: Isso vai aparecer no console do Visual Studio na hora que você logar
            foreach (var claim in claims)
            {
                Console.WriteLine($"Claim Tipo: {claim.Type} - Valor: {claim.Value}");
            }

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                new AuthenticationProperties { IsPersistent = false });

            return RedirectToAction("Index", "Dashboard");
        }

        // ================= LOGOUT =================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            // Limpa o cookie de autenticação principal
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            // Opcional: Se você usa Session, limpe-a também
            HttpContext.Session.Clear();

            // Redireciona para o Login garantindo que não há cache
            return RedirectToAction("Login", "Usuario", new { area = "" });
        }

        // ================= LISTAGEM =================

        public async Task<IActionResult> Index()
        {
            var usuarios = await _api.GetAll();
            return View(usuarios);
        }

        // ================= DETALHES =================

        public async Task<IActionResult> Details(int id)
        {
            var usuario = await _api.GetById(id);

            if (usuario == null)
                return NotFound();

            return View(usuario);
        }

        // GET: Usuario/Create
        public async Task<IActionResult> Create()
        {
            var tipos = await _tipoApi.GetAll();
            ViewBag.TipoUsuarioId = new SelectList(tipos, "IdTipoUsuario", "DescricaoTipoUsuario");
            return View(new UsuarioDto { Ativo = true });
        }

    // POST: Usuario/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(UsuarioDto usuario)
    {
        if (!ModelState.IsValid)
        {
            var tipos = await _tipoApi.GetAll();
            ViewBag.TipoUsuarioId = new SelectList(tipos, "IdTipoUsuario", "DescricaoTipoUsuario", usuario.TipoUsuarioId);
            return View(usuario);
        }

        var response = await _api.Create(usuario);

        if (!response.IsSuccessStatusCode)
        {
            var conteudo = await response.Content.ReadAsStringAsync();
            try
            {
                var erro = System.Text.Json.JsonSerializer.Deserialize<ErroDto>(conteudo,
                    new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (erro?.Mensagem?.Contains("mail") == true)
                    ModelState.AddModelError("Email", erro.Mensagem);
                else
                    ModelState.AddModelError("", erro?.Mensagem ?? "Erro ao salvar.");
            }
            catch
            {
                ModelState.AddModelError("", conteudo);
            }

            var tipos = await _tipoApi.GetAll();
            ViewBag.TipoUsuarioId = new SelectList(tipos, "IdTipoUsuario", "DescricaoTipoUsuario", usuario.TipoUsuarioId);
            return View(usuario);
        }

        return RedirectToAction(nameof(Index));
    }

    // ================= EDIT =================

    public async Task<IActionResult> Edit(int id)
        {
            var usuario = await _api.GetById(id);

            if (usuario == null)
                return NotFound();
        var tipos = await _tipoApi.GetAll();
        ViewBag.TipoUsuarioId = new SelectList(tipos, "IdTipoUsuario", "DescricaoTipoUsuario", usuario.TipoUsuarioId);

        return View(usuario);
        }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, UsuarioDto usuario)
    {
        if (id != usuario.IdUsuario)
            return NotFound();

        if (!ModelState.IsValid)
            return View(usuario);

        var response = await _api.Update(usuario);

        if (!response.IsSuccessStatusCode)
        {
            var conteudo = await response.Content.ReadAsStringAsync();
            try
            {
                var erro = System.Text.Json.JsonSerializer.Deserialize<ErroDto>(conteudo,
                    new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (erro?.Mensagem?.Contains("mail") == true)
                    ModelState.AddModelError("Email", erro.Mensagem);
                else
                    ModelState.AddModelError("", erro?.Mensagem ?? "Erro ao salvar.");
            }
            catch
            {
                ModelState.AddModelError("", conteudo);
            }
            var tipos = await _tipoApi.GetAll();
            ViewBag.TipoUsuarioId = new SelectList(tipos, "IdTipoUsuario", "DescricaoTipoUsuario", usuario.TipoUsuarioId);

            return View(usuario);
        }

        return RedirectToAction(nameof(Index));
    }

    private class ErroDto
    {
        public string Mensagem { get; set; }
    }

    // ================= DELETE =================

    [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Delete(int id)
        {
            var usuario = await _api.GetById(id);

            if (usuario == null)
                return NotFound();

            return View(usuario);
        }

        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Administrador")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _api.Delete(id);
            return RedirectToAction(nameof(Index));
        }

        // ================= INATIVOS =================

        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Inativos()
        {
            var usuarios = await _api.GetAll();
            var inativos = usuarios.Where(u => !u.Ativo)
                                   .OrderByDescending(u => u.IdUsuario)
                                   .ToList();

            return View(inativos);
        }
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Ativar(int id)
    {
        await _api.Reativar(id); // 👈 em vez de buscar e fazer Update
        return RedirectToAction(nameof(Inativos));
    }

    [AllowAnonymous] // 👈 Obrigatório, senão quem não tem acesso não vê a página!
        public IActionResult AcessoNegado()
        {
            return View();
        }
    }
