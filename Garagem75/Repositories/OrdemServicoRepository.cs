using Garagem75.Data;
using Garagem75.Interfaces;
using Garagem75.Models;
using Microsoft.EntityFrameworkCore;

namespace Garagem75.Repositories
{
    public class OrdemServicoRepository : IOrdemServicoRepository
    {
        private readonly Garagem75DBContext _context;
        public OrdemServicoRepository(Garagem75DBContext context)
        {
            _context = context;
        }
        public async Task AddAsync(OrdemServico ordemServico)
        {
            await _context.OrdemServicos.AddAsync(ordemServico);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var ordemservico = await _context.OrdemServicos.FirstOrDefaultAsync(c => c.IdOrdemServico == id);
            if (ordemservico != null)
            {
                _context.OrdemServicos.Remove(ordemservico);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<OrdemServico>> GetAllAsync()
        {
            return await _context.OrdemServicos.ToListAsync();
        }

        public async Task<OrdemServico> GetByIdAsync(int id)
        {
            return await _context.OrdemServicos.FirstOrDefaultAsync(u => u.IdOrdemServico == id);
        }

        public async Task UpdateAsync(OrdemServico ordemServico)
        {
            _context.OrdemServicos.Update(ordemServico);
            await _context.SaveChangesAsync();
        }
    }
}
