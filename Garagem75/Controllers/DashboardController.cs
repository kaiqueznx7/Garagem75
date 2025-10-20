using Garagem75.Data;
using Garagem75.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Garagem75.Controllers
{
    public class DashboardController : Controller
    {
        private readonly Garagem75DBContext _context;

        public DashboardController(Garagem75DBContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var vm = new DashboardViewModel();

            //Cards
            vm.TotalPecas = await _context.Pecas.CountAsync();
            vm.TotalClientes = await _context.Clientes.CountAsync();
            vm.TotalOrdensServico = await _context.OrdemServicos.CountAsync();
            vm.TotalUsuarios = await _context.Usuarios.CountAsync();
           // vm.ValorTotalOrdensServico = await _context.OrdemServicos.SumAsync(o => o.ValorTotal);

            //// 1. Encontra a data da última segunda-feira
            // DayOfWeek.Monday é 1, Tuesday é 2, ..., Sunday é 0.
            int diff = (7 + (int)DateTime.Now.DayOfWeek - (int)DayOfWeek.Monday) % 7;
            var inicioDaSemana = DateTime.Now.Date.AddDays(-1 * diff);

            //// 2. Filtra as ordens de serviço a partir de segunda-feira e soma o valor
            vm.ValorTotalOrdensServico = await _context.OrdemServicos
                                                 .Where(o => o.DataServico >= inicioDaSemana)
                                                 .SumAsync(o => o.ValorTotal);



            //Clientes Mais Antigos
            vm.ClientesMaisAntigos = await _context.Clientes
                .AsNoTracking()
                .OrderBy(c => c.IdCliente)
                .Select(c => new ClienteCount
                {
                    IdCliente = c.IdCliente,
                    Nome = c.Nome,
                    Telefone = c.Telefone,
                    Email = c.Email

                })
                .Take(5)
                .ToListAsync();

            //Ultimos veiculos  
            vm.UltimosVeiculosAtendidos = await _context.Veiculos
                .AsNoTracking()
                .OrderByDescending(v => v.IdVeiculo)
                .Select(v => new ModeloVeiculos
                {
                    IdVeiculo = v.IdVeiculo,
                    Fabricante = v.Fabricante,
                    Modelo = v.Modelo,
                    Placa = v.Placa
                })
                .Take(5)
                .ToListAsync();

            // 👇 Top 5 marcas de peças
            vm.MarcasPecasMaisUsadas = await _context.Pecas
                .GroupBy(p => p.Marca)
                .Select(g => new MarcaQuantidadeViewModel
                {
                    NomeMarca = g.Key,
                    Quantidade = g.Count()
                })
                .OrderByDescending(x => x.Quantidade)
                .Take(5)
                .ToListAsync();

            // 👇 Top 5 marcas de veículos
            vm.MarcasVeiculosMaisUsadas = await _context.Veiculos
                .GroupBy(v => v.Fabricante)
                .Select(g => new MarcaQuantidadeViewModel
                {
                    NomeMarca = g.Key,
                    Quantidade = g.Count()
                })
                .OrderByDescending(x => x.Quantidade)
                .Take(5)
                .ToListAsync();
            //Peças por veiculo
            vm.PecasPorVeiculo = await _context.OrdemServicos
                .AsNoTracking()
                .Select(o => new PecasPorVeiculo
                {
                    IdOrdemServico = o.IdOrdemServico,
                    Fabricante = o.Veiculo.Fabricante,
                    QuantidadePecas = o.PecasAssociadas.Count
                }) 
                .OrderByDescending(o => o.QuantidadePecas)
                .Take(5)
                .ToListAsync();


            return View(vm);
        }     
    }
}