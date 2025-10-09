using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Garagem75.Data;
using Garagem75.Models;
using Microsoft.AspNetCore.Authorization;

namespace Garagem75.Controllers
{
    [Authorize(Roles = "Administrador, Mêcanico")]
    public class OrdemServicosController : Controller
    {
        private readonly Garagem75DBContext _context;

        public OrdemServicosController(Garagem75DBContext context)
        {
            _context = context;
        }

        // GET: OrdemServicos
        public async Task<IActionResult> Index()
        {
            return View(await _context.OrdemServicos.ToListAsync());
        }

        // GET: OrdemServicos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ordemServico = await _context.OrdemServicos
                .FirstOrDefaultAsync(m => m.IdOrdemServico == id);
            if (ordemServico == null)
            {
                return NotFound();
            }

            return View(ordemServico);
        }

        // GET: OrdemServicos/Create
        public IActionResult Create()
        {
            ViewData["VeiculoId"] = new SelectList(_context.Veiculos, "IdVeiculo", "Modelo");
            return View();
        }


        // POST: OrdemServicos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdOrdemServico,Descricao,DataServico,MaoDeObra,ValorDesconto,ValorTotal,Status,DataEntrega,VeiculoId")] OrdemServico ordemServico)
        {
            if (ModelState.IsValid)
            {
                _context.Add(ordemServico);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["VeiculoId"] = new SelectList(_context.Veiculos, "IdVeiculo", "Modelo", ordemServico.VeiculoId);
            return View(ordemServico);
        }

        // GET: OrdemServicos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ordemServico = await _context.OrdemServicos.FindAsync(id);
            if (ordemServico == null)
            {
                return NotFound();
            }
            return View(ordemServico);
        }

        // POST: OrdemServicos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdOrdemServico,Descricao,DataServico,MaoDeObra,ValorDesconto,ValorTotal,Status,DataEntrega,VeiculoId")] OrdemServico ordemServico)
        {
            if (id != ordemServico.IdOrdemServico)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(ordemServico);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrdemServicoExists(ordemServico.IdOrdemServico))
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
            return View(ordemServico);
        }

        // GET: OrdemServicos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ordemServico = await _context.OrdemServicos
                .FirstOrDefaultAsync(m => m.IdOrdemServico == id);
            if (ordemServico == null)
            {
                return NotFound();
            }

            return View(ordemServico);
        }

        // POST: OrdemServicos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ordemServico = await _context.OrdemServicos.FindAsync(id);
            if (ordemServico != null)
            {
                _context.OrdemServicos.Remove(ordemServico);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrdemServicoExists(int id)
        {
            return _context.OrdemServicos.Any(e => e.IdOrdemServico == id);
        }
    }
}
