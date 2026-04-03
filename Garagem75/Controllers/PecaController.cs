using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Garagem75.Client.Services;
using Garagem75.Shared.Dtos;
using System.Globalization;

namespace Garagem75.Controllers
{
    [Authorize(Roles = "Administrador, Mêcanico")]
    public class PecaController : Controller
    {
        private readonly PecaApiService _api;

        public PecaController(PecaApiService api)
        {
            _api = api;
        }

        // ================= INDEX =================
        public async Task<IActionResult> Index(string marca, string searchString)
        {
            var pecas = await _api.GetAll();

            // 🔥 montar marcas (igual antes)
            var marcas = pecas
                .Select(p => p.Marca)
                .Distinct()
                .OrderBy(m => m)
                .ToList();

            ViewBag.Marcas = new SelectList(marcas);

            // filtros
            if (!string.IsNullOrEmpty(marca))
                pecas = pecas.Where(p => p.Marca == marca).ToList();

            if (!string.IsNullOrEmpty(searchString))
                pecas = pecas.Where(p =>
                    p.Nome.Contains(searchString) ||
                    p.Fornecedor.Contains(searchString)
                ).ToList();

            return View(pecas);
        }

        // ================= DETAILS =================
        public async Task<IActionResult> Details(int id)
        {
            var peca = await _api.GetById(id);
            if (peca == null) return NotFound();

            return View(peca);
        }

        // ================= CREATE =================
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PecaDto dto, IFormFile? ImagemUpload)
        {
            // 🔥 tratar preço
            if (Request.Form.TryGetValue("Preco", out var precoTexto))
            {
                try
                {
                    precoTexto = precoTexto.ToString()
                        .Replace("R$", "")
                        .Replace(".", "")
                        .Replace(",", ".");

                    dto.Preco = decimal.Parse(precoTexto, CultureInfo.InvariantCulture);
                }
                catch
                {
                    ModelState.AddModelError("Preco", "Formato inválido");
                }
            }

            if (!ModelState.IsValid)
                return View(dto);

            // 🚀 1. CRIA PEÇA (SEM IMAGEM)
            var criada = await _api.Create(dto);

            // 🚀 2. UPLOAD DA IMAGEM (se tiver)
            if (ImagemUpload != null && ImagemUpload.Length > 0)
            {
                await _api.UploadImagem(criada.IdPeca, ImagemUpload);
            }

            return RedirectToAction(nameof(Index));
        }

        // ================= EDIT =================
        public async Task<IActionResult> Edit(int id)
        {
            var peca = await _api.GetById(id);
            if (peca == null) return NotFound();

            return View(peca);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PecaDto dto, IFormFile? ImagemUpload)
        {
            if (id != dto.IdPeca)
                return NotFound();

            // 🔥 preço
            if (Request.Form.TryGetValue("Preco", out var precoTexto))
            {
                try
                {
                    precoTexto = precoTexto.ToString()
                        .Replace("R$", "")
                        .Replace(".", "")
                        .Replace(",", ".");

                    dto.Preco = decimal.Parse(precoTexto, CultureInfo.InvariantCulture);
                }
                catch
                {
                    ModelState.AddModelError("Preco", "Formato inválido");
                }
            }

            if (!ModelState.IsValid)
                return View(dto);

            // 🔥 imagem
            if (ImagemUpload != null && ImagemUpload.Length > 0)
            {
                var dir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img");
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

                var fileName = Path.GetFileName(ImagemUpload.FileName);
                var filePath = Path.Combine(dir, fileName);

                using var stream = new FileStream(filePath, FileMode.Create);
                await ImagemUpload.CopyToAsync(stream);

                dto.Imagem = "/img/" + fileName;
            }

            

            await _api.Update(dto);

            return RedirectToAction(nameof(Index));
        }

        // ================= DELETE =================
        public async Task<IActionResult> Delete(int id)
        {
            var peca = await _api.GetById(id);
            if (peca == null) return NotFound();

            return View(peca);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _api.Delete(id);
            return RedirectToAction(nameof(Index));
        }
    }
}