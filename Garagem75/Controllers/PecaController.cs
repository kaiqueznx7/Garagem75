using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Garagem75.Data;
using Garagem75.Models;
using Microsoft.DotNet.Scaffolding.Shared.CodeModifier.CodeChange;
using Microsoft.AspNetCore.Authorization;
using System.Globalization;

namespace Garagem75.Controllers
{
    [Authorize(Roles = "Administrador, Mêcanico")]
    public class PecaController : Controller
    {
        private readonly Garagem75DBContext _context;

        public PecaController(Garagem75DBContext context)
        {
            _context = context;
        }

        // GET: Peca
        public async Task<IActionResult> Index(string marca, string searchString)
        {
            // Buscar marcas distintas no banco
            var marcas = await _context.Pecas
                                       .Select(p => p.Marca)
                                       .Distinct()
                                       .OrderBy(m => m)
                                       .ToListAsync();

            ViewBag.Marcas = new SelectList(marcas);

            // Query base
            var pecas = from p in _context.Pecas
                        select p;

            // Filtro por marca
            if (!string.IsNullOrEmpty(marca))
            {
                pecas = pecas.Where(p => p.Marca == marca);
            }

            // Filtro por nome da peça
            if (!string.IsNullOrEmpty(searchString))
            {
                pecas = pecas.Where(p => p.Nome.Contains(searchString)
                                      || p.Fornecedor.Contains(searchString));
            }

            return View(await pecas.ToListAsync());
        }



        // GET: Peca/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var peca = await _context.Pecas
                .FirstOrDefaultAsync(m => m.IdPeca == id);
            if (peca == null)
            {
                return NotFound();
            }

            return View(peca);
        }

        // GET: Peca/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Peca/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Peca peca, IFormFile? ImagemUpload)
        {
           

            // Preço seguro
            if (Request.Form.TryGetValue("Preco", out var precoTexto))
            {
                try
                {
                    precoTexto = precoTexto.ToString()
                                           .Replace("R$", "")
                                           .Replace(".", "")
                                           .Replace(",", ".");
                    peca.Preco = decimal.Parse(precoTexto, CultureInfo.InvariantCulture);
                }
                catch
                {
                    ModelState.AddModelError("Preco", "Formato de preço inválido!");
                }
            }

            if (ModelState.IsValid)
            {
                // 🚨 AÇÃO CRÍTICA: Preencher DataCadastro com a data e hora atual do servidor
                peca.DataCadastro = DateTime.Now;

                // Se você também tiver DataUltimaAtualizacao, preencha-a aqui também:
                peca.DataUltimaAtualizacao = DateTime.Now;
                if (ImagemUpload != null && ImagemUpload.Length > 0)
                {
                    var dir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img");
                    if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

                    var fileName = Path.GetFileName(ImagemUpload.FileName);
                    var filePath = Path.Combine(dir, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await ImagemUpload.CopyToAsync(stream);
                    }

                    peca.Imagem = "/img/" + fileName;
                }

                _context.Add(peca);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(peca);
        }


        // GET: Peca/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var peca = await _context.Pecas.FindAsync(id);
            if (peca == null)
            {
                return NotFound();
            }
            return View(peca);
        }

        // POST: Peca/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Peca peca, IFormFile? ImagemUpload)
        {
            if (id != peca.IdPeca) return NotFound();

            // 🔹 Corrige o preço para decimal
            if (Request.Form.TryGetValue("Preco", out var precoTexto))
            {
                try
                {
                    precoTexto = precoTexto.ToString()
                                           .Replace("R$", "")
                                           .Replace(".", "")
                                           .Replace(",", ".");
                    peca.Preco = decimal.Parse(precoTexto, CultureInfo.InvariantCulture);
                }
                catch
                {
                    ModelState.AddModelError("Preco", "Formato de preço inválido!");
                }
            }

            if (ModelState.IsValid)
            {
                var pecaDb = await _context.Pecas.FindAsync(id);
                if (pecaDb == null) return NotFound();

                // Atualiza campos
                pecaDb.CodPeca = peca.CodPeca;
                pecaDb.Marca = peca.Marca;
                pecaDb.Nome = peca.Nome;
                pecaDb.Preco = peca.Preco;
                pecaDb.Fornecedor = peca.Fornecedor;
                pecaDb.QuantidadeEstoque = peca.QuantidadeEstoque;
                pecaDb.DataUltimaAtualizacao = DateTime.Now;

                // Atualiza imagem se enviada
                if (ImagemUpload != null && ImagemUpload.Length > 0)
                {
                    var dir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img");
                    if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

                    var fileName = Path.GetFileName(ImagemUpload.FileName);
                    var filePath = Path.Combine(dir, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await ImagemUpload.CopyToAsync(stream);
                    }

                    pecaDb.Imagem = "/img/" + fileName;
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(peca);
        }




        // GET: Peca/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var peca = await _context.Pecas
                .FirstOrDefaultAsync(m => m.IdPeca == id);
            if (peca == null)
            {
                return NotFound();
            }

            return View(peca);
        }

        // POST: Peca/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var peca = await _context.Pecas.FindAsync(id);
            if (peca != null)
            {
                _context.Pecas.Remove(peca);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PecaExists(int id)
        {
            return _context.Pecas.Any(e => e.IdPeca == id);
        }
    }
}
