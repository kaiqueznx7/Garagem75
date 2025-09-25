using Garagem75.Data;
using Garagem75.Interfaces;
using Garagem75.Models;
using Microsoft.EntityFrameworkCore;

namespace Garagem75.Repositories
{
    public class VeiculoRepository : IVeiculoRepository
    {
        private readonly Garagem75DBContext _context;
        public VeiculoRepository(Garagem75DBContext context)
        {
            _context = context;
        }
        public async Task AddAsync(Veiculo veiculo)
        {
            await _context.Veiculos.AddAsync(veiculo);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var veiculo = await _context.Veiculos.FirstOrDefaultAsync(c => c.IdVeiculo == id);
            if (veiculo != null)
            {
                _context.Veiculos.Remove(veiculo);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<Veiculo>> GetAllAsync()
        {
            return await _context.Veiculos.ToListAsync();
        }

        public async Task<Veiculo> GetByIdAsync(int id)
        {
            return await _context.Veiculos.FirstOrDefaultAsync(u => u.IdVeiculo == id);
        }

        public async Task UpdateAsync(Veiculo veiculo)
        {
            _context.Veiculos.Update(veiculo);
            await _context.SaveChangesAsync();
        }
    }
}
