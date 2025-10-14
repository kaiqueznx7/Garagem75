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
                .Take(2)
                .ToListAsync();

            //Marcas de Peças Mais Usadas
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


            return View(vm);
        }     
    }
}