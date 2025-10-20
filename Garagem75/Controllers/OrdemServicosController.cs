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
            // CORREÇÃO: Uso de PecasAssociadas
            return View(await _context.OrdemServicos
                .Include(o => o.Veiculo)
                .Include(o => o.PecasAssociadas)
                    .ThenInclude(op => op.Peca)
                .ToListAsync());
        }

        // GET: OrdemServicos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // CORREÇÃO: Uso de PecasAssociadas
            var ordemServico = await _context.OrdemServicos
                .Include(o => o.PecasAssociadas)
                    .ThenInclude(op => op.Peca)
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
            var veiculos = _context.Veiculos
                .Select(v => new
                {
                    v.IdVeiculo,
                    NomeCompleto = v.Modelo + " - " + v.Placa
                })
                .ToList();

            ViewData["VeiculoId"] = new SelectList(veiculos, "IdVeiculo", "NomeCompleto");
            ViewData["Pecas"] = _context.Pecas.ToList();
            return View();
        }

        // POST: OrdemServicos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("IdOrdemServico,Descricao,DataServico,MaoDeObra,ValorDesconto,ValorTotal,Status,DataEntrega,VeiculoId")] OrdemServico ordemServico,
            int[] pecaIds, // IDs das peças selecionadas
            int[] quantidades) // Quantidades correspondentes
        {
            if (ModelState.IsValid)
            {
                if (pecaIds.Length != quantidades.Length)
                {
                    ModelState.AddModelError("", "A lista de peças e quantidades não corresponde.");
                    // Não precisa do `return View(ordemServico);` aqui, pois o retorno está no final do método.
                }

                // 1. Inicializa e calcula o total
                decimal totalPecas = 0;
                ordemServico.PecasAssociadas = new List<OrdemServicoPeca>();

                for (int i = 0; i < pecaIds.Length; i++)
                {
                    var pecaId = pecaIds[i];
                    var quantidade = quantidades[i];

                    if (quantidade > 0)
                    {
                        // Encontra a peça SEM rastreamento, ou usa FindAsync, mas usaremos apenas a PecaId
                        var peca = await _context.Pecas.AsNoTracking().FirstOrDefaultAsync(p => p.IdPeca == pecaId);

                        if (peca != null)
                        {
                            var novaAssociacao = new OrdemServicoPeca
                            {
                                PecaId = pecaId, // Usa a chave estrangeira em vez do objeto de navegação
                                Quantidade = quantidade,
                                PrecoUnitario = peca.Preco
                            };
                            ordemServico.PecasAssociadas.Add(novaAssociacao);
                            totalPecas += novaAssociacao.Subtotal;
                        }
                    }
                }

                // 2. Calcula o ValorTotal
                ordemServico.ValorTotal = ordemServico.MaoDeObra + totalPecas - ordemServico.ValorDesconto;

                _context.Add(ordemServico);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Código de retorno em caso de falha de validação
            var veiculos = _context.Veiculos
                .Select(v => new
                {
                    v.IdVeiculo,
                    NomeCompleto = v.Modelo + " - " + v.Placa
                })
                .ToList();

            ViewData["VeiculoId"] = new SelectList(veiculos, "IdVeiculo", "NomeCompleto", ordemServico.VeiculoId);
            ViewData["Pecas"] = _context.Pecas.ToList();
            return View(ordemServico);
        }

        // GET: OrdemServicos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // CORREÇÃO: Incluir PecasAssociadas e a Peca
            var ordemServico = await _context.OrdemServicos
                .Include(o => o.PecasAssociadas)
                    .ThenInclude(op => op.Peca)
                .FirstOrDefaultAsync(m => m.IdOrdemServico == id);

            if (ordemServico == null)
            {
                return NotFound();
            }

            var veiculos = _context.Veiculos
                .Select(v => new
                {
                    v.IdVeiculo,
                    NomeCompleto = v.Modelo + " - " + v.Placa
                })
                .ToList();

            ViewData["VeiculoId"] = new SelectList(veiculos, "IdVeiculo", "NomeCompleto", ordemServico.VeiculoId);
            ViewData["Pecas"] = _context.Pecas.ToList();
            return View(ordemServico);
        }

        // POST: OrdemServicos/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            [Bind("IdOrdemServico,Descricao,DataServico,MaoDeObra,ValorDesconto,ValorTotal,Status,DataEntrega,VeiculoId")] OrdemServico ordemServico,
            int[] pecaIds,
            int[] quantidades)
        {
            if (id != ordemServico.IdOrdemServico)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                // Verifica inconsistência de dados
                if (pecaIds.Length != quantidades.Length)
                {
                    ModelState.AddModelError("", "A lista de peças e quantidades não corresponde.");
                    // Não retorna aqui, cai no bloco de retorno da View no final do método.
                }

                // 1. Carrega a Ordem de Serviço do banco, incluindo AS ASSOCIAÇÕES
                var ordemServicoAtual = await _context.OrdemServicos
                    .Include(o => o.PecasAssociadas)
                    .FirstOrDefaultAsync(o => o.IdOrdemServico == id);

                if (ordemServicoAtual == null)
                {
                    return NotFound();
                }

                // 2. Atualiza as propriedades simples
                _context.Entry(ordemServicoAtual).CurrentValues.SetValues(ordemServico);

                // 3. Limpa a coleção existente de associações (remove todas as peças existentes)
                ordemServicoAtual.PecasAssociadas.Clear();

                // 4. Reconstroi as peças associadas com a quantidade e preço
                decimal totalPecas = 0;

                for (int i = 0; i < pecaIds.Length; i++)
                {
                    var pecaId = pecaIds[i];
                    var quantidade = quantidades[i];

                    if (quantidade > 0)
                    {
                        var peca = await _context.Pecas.AsNoTracking().FirstOrDefaultAsync(p => p.IdPeca == pecaId);

                        if (peca != null)
                        {
                            var novaAssociacao = new OrdemServicoPeca
                            {
                                PecaId = pecaId, // Usa a chave estrangeira em vez do objeto de navegação
                                Quantidade = quantidade,
                                PrecoUnitario = peca.Preco
                            };

                            ordemServicoAtual.PecasAssociadas.Add(novaAssociacao);
                            totalPecas += novaAssociacao.Subtotal;
                        }
                    }
                }

                // 5. Calcula o novo ValorTotal
                ordemServicoAtual.ValorTotal = ordemServicoAtual.MaoDeObra + totalPecas - ordemServicoAtual.ValorDesconto;

                try
                {
                    await _context.SaveChangesAsync();
                }
                // CORREÇÃO: Bloco Catch obrigatório para o Try
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrdemServicoExists(ordemServicoAtual.IdOrdemServico))
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

            // Código de retorno em caso de falha de validação ou ID mismatch
            var veiculos = _context.Veiculos
                .Select(v => new
                {
                    v.IdVeiculo,
                    NomeCompleto = v.Modelo + " - " + v.Placa
                })
                .ToList();

            ViewData["VeiculoId"] = new SelectList(veiculos, "IdVeiculo", "NomeCompleto", ordemServico.VeiculoId);
            ViewData["Pecas"] = _context.Pecas.ToList();
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

        public async Task<IActionResult> Relatorio(int? id)
        {
            if (id == null)
                return NotFound();

            var ordemServico = await _context.OrdemServicos              
                .Include(o => o.PecasAssociadas)
                    .ThenInclude(op => op.Peca)
                .FirstOrDefaultAsync(o => o.IdOrdemServico == id);

            if (ordemServico == null)
                return NotFound();

            return View(ordemServico);
        }


        // POST: OrdemServicos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // Nota: Se a OrdemServico for excluída, o EF Core
            // deve remover automaticamente as entradas em OrdemServicoPeca
            // devido à configuração em cascata (cascading delete) que é 
            // padrão para relacionamentos necessários.
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