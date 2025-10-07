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
            // Log para conferir se o arquivo chegou
            Console.WriteLine($"Arquivo recebido: {ImagemUpload?.FileName ?? "NENHUM"}");

            if (ModelState.IsValid)
            {
                if (ImagemUpload != null && ImagemUpload.Length > 0)
                {
                    // Pasta wwwroot/img
                    var dir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img");
                    if (!Directory.Exists(dir))
                        Directory.CreateDirectory(dir);

                    var fileName = Path.GetFileName(ImagemUpload.FileName);
                    var filePath = Path.Combine(dir, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await ImagemUpload.CopyToAsync(stream);
                    }

                    // Salva o caminho relativo no banco
                    peca.Imagem = "/img/" + fileName;
                }

                // Salva no banco
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
            if (id != peca.IdPeca)
                return NotFound();

            if (!ModelState.IsValid)
            {
                var erros = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                Console.WriteLine("Erros de validação: " + string.Join(", ", erros));
                return View(peca);
            }

            try
            {
                var pecaDb = await _context.Pecas.FindAsync(id);
                if (pecaDb == null) return NotFound();

                // Atualiza campos básicos
                pecaDb.CodPeca = peca.CodPeca;
                pecaDb.Marca = peca.Marca;
                pecaDb.Nome = peca.Nome;
                pecaDb.Preco = peca.Preco;
                pecaDb.Fornecedor = peca.Fornecedor;
                pecaDb.QuantidadeEstoque = peca.QuantidadeEstoque;
                pecaDb.DataUltimaAtualizacao = DateTime.Now;

                // Se enviou nova imagem, salva
                if (ImagemUpload != null && ImagemUpload.Length > 0)
                {
                    var dir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img");
                    if (!Directory.Exists(dir))
                        Directory.CreateDirectory(dir);

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
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Pecas.Any(e => e.IdPeca == peca.IdPeca))
                    return NotFound();
                else
                    throw;
            }
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
