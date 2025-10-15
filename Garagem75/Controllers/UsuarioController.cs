using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Garagem75.Data;
using Garagem75.Models;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Garagem75.Interfaces;
using Garagem75.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace Garagem75.Controllers
{
    
    public class UsuarioController : Controller
    {
        private readonly Garagem75DBContext _context;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly ITipoUsuarioRepository _tipoUsuarioRepository;


        public UsuarioController(Garagem75DBContext context, IUsuarioRepository usuarioRepository,
            ITipoUsuarioRepository tipoUsuarioRepository)
        {
            _context = context;
            _usuarioRepository = usuarioRepository;
            _tipoUsuarioRepository = tipoUsuarioRepository;
        }



        // GET: Usuario/Login
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string? returnUrl = null)
        {
            if (User?.Identity?.IsAuthenticated == true)
                return RedirectToAction("Index", "Usuario"); // se já logado, vai para a lista

            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        // POST: Usuario/Login
    [HttpPost]
    public async Task<IActionResult> Login(string email, string senha)
    {
        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(senha))
        {
            ViewBag.Error = "Preencha todos os campos.";
            return View();
        }

        var usuario = await _context.Usuarios
            .Include(u => u.TipoUsuario)
            .FirstOrDefaultAsync(u => u.Email == email && u.Senha == senha);

        if (usuario == null)
        {
                ModelState.AddModelError(string.Empty, "Email ou senha inválidos.");
                return View();
            }

        // Cria as claims
        var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, usuario.Nome),
        new Claim(ClaimTypes.Email, usuario.Email),
        new Claim(ClaimTypes.Role, usuario.TipoUsuario.DescricaoTipoUsuario)
    };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
        ViewBag.Success = "Login realizado com sucesso!";


            return RedirectToAction("Index", "Usuario");
    }


        // GET: Usuario/Logout
        [HttpGet]
        public async Task<IActionResult> LogoutGet()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            // 🔧 apaga o cookie de antiforgery (nome deve bater com o Program.cs)
            Response.Cookies.Delete("Garagem75.AntiCsrf");

            // 🔧 volta pra Home com um cache-buster pra forçar HTML/JS novos
            return RedirectToAction("Index", "Home", new { _ts = DateTimeOffset.UtcNow.ToUnixTimeSeconds() });
        }

        // GET: Usuario
        public async Task<IActionResult> Index()
        {
            var garagem75DBContext = _context.Usuarios.Include(u => u.TipoUsuario);
            return View(await garagem75DBContext.ToListAsync());
        }

        // GET: Usuario/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuarios
                .Include(u => u.TipoUsuario)
                .FirstOrDefaultAsync(m => m.IdUsuario == id);
            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario);
        }

        // GET: Usuario/Create
        public IActionResult Create()
        {
            ViewData["TipoUsuarioId"] = new SelectList(_context.TipoUsuarios, "IdTipoUsuario", "DescricaoTipoUsuario");
            return View();
        }

        // POST: Usuario/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdUsuario,Nome,Email,Senha,TipoUsuarioId,Ativo")] Usuario usuario)
        {
            if (ModelState.IsValid)
            {
                _context.Add(usuario);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["TipoUsuarioId"] = new SelectList(_context.TipoUsuarios, "IdTipoUsuario", "DescricaoTipoUsuario", usuario.TipoUsuarioId);
            return View(usuario);
        }

        // GET: Usuario/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }
            ViewData["TipoUsuarioId"] = new SelectList(_context.TipoUsuarios, "IdTipoUsuario", "DescricaoTipoUsuario", usuario.TipoUsuarioId);
            return View(usuario);
        }

        // POST: Usuario/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdUsuario,Nome,Email,Senha,TipoUsuarioId,Ativo")] Usuario usuario)
        {
            if (id != usuario.IdUsuario)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(usuario);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UsuarioExists(usuario.IdUsuario))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["TipoUsuarioId"] = new SelectList(_context.TipoUsuarios, "IdTipoUsuario", "DescricaoTipoUsuario", usuario.TipoUsuarioId);
            return View(usuario);
        }

        // GET: Usuario/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuarios
                .Include(u => u.TipoUsuario)
                .FirstOrDefaultAsync(m => m.IdUsuario == id);
            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario);
        }

        // POST: Usuario/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario != null)
            {
                _context.Usuarios.Remove(usuario);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UsuarioExists(int id)
        {
            return _context.Usuarios.Any(e => e.IdUsuario == id);
        }
        private static string NormalizeRole(string? raw)
        {
            var r = (raw ?? "").Trim().ToLowerInvariant();
            return r switch
            {
                "Administrador" or "admin" => "Administrador",
                "Mecânico" or "mecanico" => "Mêcanico",
                _ => "Outros"
            };
        }
    }
}
