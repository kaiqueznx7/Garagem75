using Garagem75.Client.Services;
using Garagem75.Services;
using Garagem75.Shared.Dtos;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

namespace Garagem75.Controllers
{
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

            if (response == null || string.IsNullOrEmpty(response.Token))
            {
                ModelState.AddModelError("", "Email ou senha inválidos.");
                return View();
            }

            // 2. Cria as Claims de autenticação do MVC
            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, response.Nome ?? ""),
        new Claim(ClaimTypes.Role, response.Role ?? ""),
        // CRUCIAL: Salva o token da API no Cookie do MVC
        new Claim("JWToken", response.Token)
    };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                new AuthenticationProperties { IsPersistent = true });

            return RedirectToAction("Index", "Dashboard");
        }

        // ================= LOGOUT =================

        [HttpGet]
        public async Task<IActionResult> LogoutGet()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            Response.Cookies.Delete("Garagem75.AntiCsrf");

            return RedirectToAction("Index", "Home",
                new { _ts = DateTimeOffset.UtcNow.ToUnixTimeSeconds() });
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

            await _api.Create(usuario);
            return RedirectToAction(nameof(Index));
        }

        // ================= EDIT =================

        public async Task<IActionResult> Edit(int id)
        {
            var usuario = await _api.GetById(id);

            if (usuario == null)
                return NotFound();

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

            await _api.Update(usuario);
            return RedirectToAction(nameof(Index));
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
            var usuario = await _api.GetById(id);

            if (usuario == null)
                return NotFound();

            usuario.Ativo = true;
            await _api.Update(usuario);

            return RedirectToAction(nameof(Inativos));
        }
    }
}