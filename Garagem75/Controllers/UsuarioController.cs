using Garagem75.Shared.Dtos;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Garagem75.Client.Services;

namespace Garagem75.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly UsuarioApiService _api;

        public UsuarioController(UsuarioApiService api)
        {
            _api = api;
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
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(senha))
            {
                ViewBag.Error = "Preencha todos os campos.";
                return View();
            }

            var usuario = await _api.Login(email, senha);

            if (usuario == null)
            {
                ModelState.AddModelError("", "Email ou senha inválidos.");
                return View();
            }


            var claims = new List<Claim>
            {

                new Claim(ClaimTypes.Name, usuario.Nome ?? ""),
                new Claim(ClaimTypes.Role, usuario.Tipo ?? ""),
                
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTime.UtcNow.AddHours(2)
                });

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

        // ================= CREATE =================

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UsuarioDto usuario)
        {
            if (!ModelState.IsValid)
                return View(usuario);

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