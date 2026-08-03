using Garagem75.Api.Data;
using Garagem75.Shared.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Garagem75.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DashboardController : ControllerBase
    {
        private readonly Garagem75DBContext _context;

        public DashboardController(Garagem75DBContext context)
        {
            _context = context;
        }

        [HttpGet("faturamento-dia")]
        public async Task<ActionResult<decimal>> GetFaturamentoDia()
        {
            try
            {
                var hojeInicio = DateTime.Today;
                var hojeFim = hojeInicio.AddDays(1);

                // Busca sem o Include primeiro para evitar ciclos de referência ou falhas na junção (JOIN)
                var ordensDoDia = await _context.OrdemServicos
                    .Where(x => x.DataServico >= hojeInicio && x.DataServico < hojeFim)
                    .ToListAsync();

                if (ordensDoDia == null || !ordensDoDia.Any())
                {
                    return Ok(0m);
                }

                // Se ValorTotal já for uma propriedade calculada/salva no banco da OS, usamos direto:
                var total = ordensDoDia.Sum(x => x.ValorTotal);

                return Ok(total);
            }
            catch (Exception ex)
            {
                // Esse log vai imprimir o erro exato no terminal do seu Backend
                Console.WriteLine($"[ERRO FATURAMENTO DIA]: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"[INNER ERROR]: {ex.InnerException.Message}");
                }

                // Retorna status 200 com valor zero para não travar a tela do cliente Blazor
                return Ok(0m);
            }
        }
        // 🚗 FABRICANTES
        [HttpGet("fabricantes")]
        public async Task<IActionResult> GetFabricantes()
        {
            var dados = await _context.Veiculos
                .GroupBy(v => v.Fabricante)
                .Select(g => new
                {
                    nome = g.Key ?? "Não Informado",
                    total = g.Count()
                })
                .ToListAsync();

            return Ok(dados);
        }

        // 🔧 MARCAS DE PEÇAS
        [HttpGet("marcas-pecas")]
        public async Task<IActionResult> GetMarcasPecas()
        {
            var dados = await _context.Pecas
                .GroupBy(p => p.Marca)
                .Select(g => new
                {
                    nome = g.Key ?? "Não Informado",
                    total = g.Count()
                })
                .ToListAsync();

            return Ok(dados);
        }

        [HttpGet]
        public async Task<ActionResult<DashboardDto>> GetDashboard()
        {
            var dto = new DashboardDto();

            // 1. CARDS BÁSICOS
            try { dto.TotalPecas = await _context.Pecas.CountAsync(); } catch { dto.TotalPecas = 0; }
            try { dto.TotalClientes = await _context.Clientes.CountAsync(); } catch { dto.TotalClientes = 0; }
            try { dto.TotalOrdensServico = await _context.OrdemServicos.CountAsync(); } catch { dto.TotalOrdensServico = 0; }
            try { dto.TotalUsuarios = _context.Usuarios != null ? await _context.Usuarios.CountAsync() : 0; } catch { dto.TotalUsuarios = 0; }

            // 2. CÁLCULO DA SEMANA
            try
            {
                int diff = (7 + (int)DateTime.Now.DayOfWeek - (int)DayOfWeek.Monday) % 7;
                var inicioDaSemana = DateTime.Now.Date.AddDays(-diff);

                dto.ValorTotalOrdensServico = await _context.OrdemServicos
                    .Where(o => o.DataServico >= inicioDaSemana)
                    .SumAsync(o => (decimal?)o.ValorTotal) ?? 0m;
            }
            catch
            {
                dto.ValorTotalOrdensServico = 0m;
            }

            // 3. LISTAS SECUNDÁRIAS (Com Null Checks)
            try
            {
                dto.ClientesMaisAntigos = await _context.Clientes
                    .OrderBy(c => c.IdCliente)
                    .Take(5)
                    .Select(c => new ClienteCount
                    {
                        IdCliente = c.IdCliente,
                        Nome = c.Nome ?? "",
                        Telefone = c.Telefone ?? "",
                        Email = c.Email ?? ""
                    }).ToListAsync();
            }
            catch { dto.ClientesMaisAntigos = new(); }

            try
            {
                dto.UltimosVeiculosAtendidos = await _context.Veiculos
                    .OrderByDescending(v => v.IdVeiculo)
                    .Take(5)
                    .Select(v => new ModeloVeiculos
                    {
                        IdVeiculo = v.IdVeiculo,
                        Fabricante = v.Fabricante ?? "",
                        Modelo = v.Modelo ?? "",
                        Placa = v.Placa ?? ""
                    }).ToListAsync();
            }
            catch { dto.UltimosVeiculosAtendidos = new(); }

            try
            {
                dto.UltimasPecas = await _context.Pecas
                    .OrderByDescending(p => p.IdPeca)
                    .Take(5)
                    .Select(p => new PecaItem
                    {
                        IdPeca = p.IdPeca,
                        Nome = p.Nome ?? "",
                        Marca = p.Marca ?? "",
                        Preco = p.Preco
                    }).ToListAsync();
            }
            catch { dto.UltimasPecas = new(); }

            try
            {
                dto.MarcasPecasMaisUsadas = await _context.Pecas
                    .GroupBy(p => p.Marca)
                    .Select(g => new MarcaQuantidadeViewModel
                    {
                        NomeMarca = g.Key ?? "Outras",
                        Quantidade = g.Count()
                    })
                    .OrderByDescending(x => x.Quantidade)
                    .Take(5)
                    .ToListAsync();
            }
            catch { dto.MarcasPecasMaisUsadas = new(); }

            try
            {
                dto.MarcasVeiculosMaisUsadas = await _context.Veiculos
                    .GroupBy(v => v.Fabricante)
                    .Select(g => new MarcaQuantidadeViewModel
                    {
                        NomeMarca = g.Key ?? "Outras",
                        Quantidade = g.Count()
                    })
                    .OrderByDescending(x => x.Quantidade)
                    .Take(5)
                    .ToListAsync();
            }
            catch { dto.MarcasVeiculosMaisUsadas = new(); }

            // 4. PEÇAS POR VEÍCULO (Checagem estrita para não quebrar se Veiculo for null)
            try
            {
                dto.PecasPorVeiculo = await _context.OrdemServicos
       .OrderByDescending(o => o.PecasAssociadas.Count)
       .Take(5)
       .Select(o => new PecasPorVeiculo
       {
           IdOrdemServico = o.IdOrdemServico,
           Modelo = o.Veiculo != null ? o.Veiculo.Modelo : "N/A",
           QuantidadePecas = o.PecasAssociadas != null ? o.PecasAssociadas.Count : 0
       })
       .ToListAsync();
            }
            catch { dto.PecasPorVeiculo = new(); }

            return Ok(dto);
        }
    }
}