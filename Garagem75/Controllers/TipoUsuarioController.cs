using Garagem75.Client.Services;
using Garagem75.Services;
using Garagem75.Shared.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Garagem75.Controllers
{
    [Authorize(Roles = "Administrador")] // Geralmente apenas Admin mexe em níveis de acesso
    public class TipoUsuarioController : Controller
    {
        private readonly TipoUsuarioApiService _api;

        public TipoUsuarioController(TipoUsuarioApiService api)
        {
            _api = api;
        }

        // GET: TipoUsuario
        public async Task<IActionResult> Index()
        {
            var tipos = await _api.GetAll();
            return View(tipos);
        }

        // GET: TipoUsuario/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var tipoUsuario = await _api.GetById(id);
            if (tipoUsuario == null) return NotFound();

            return View(tipoUsuario);
        }

        // GET: TipoUsuario/Create
        public IActionResult Create() => View(new TipoUsuarioDto());

        // POST: TipoUsuario/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TipoUsuarioDto dto)
        {
            if (ModelState.IsValid)
            {
                await _api.Create(dto);
                return RedirectToAction(nameof(Index));
            }
            return View(dto);
        }

        // GET: TipoUsuario/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var tipoUsuario = await _api.GetById(id);
            if (tipoUsuario == null) return NotFound();

            return View(tipoUsuario);
        }

        // POST: TipoUsuario/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TipoUsuarioDto dto)
        {
            if (id != dto.IdTipoUsuario) return NotFound();

            if (ModelState.IsValid)
            {
                await _api.Update(dto);
                return RedirectToAction(nameof(Index));
            }
            return View(dto);
        }

        // GET: TipoUsuario/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var tipoUsuario = await _api.GetById(id);
            if (tipoUsuario == null) return NotFound();

            return View(tipoUsuario);
        }

        // POST: TipoUsuario/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _api.Delete(id);
            return RedirectToAction(nameof(Index));
        }
    }
}