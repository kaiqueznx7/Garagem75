using Garagem75.Data;
using Garagem75.Interfaces;
using Garagem75.Models;
using Microsoft.EntityFrameworkCore;

namespace Garagem75.Repositories
{
    public class PecaRepository : IPecaRepository
    {
        private readonly Garagem75DBContext _context;
        public PecaRepository(Garagem75DBContext context)
        {
            _context = context;
        }
        public async Task AddAsync(Peca peca)
        {
            await _context.Pecas.AddAsync(peca);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var peca = await _context.Pecas.FirstOrDefaultAsync(c => c.IdPeca == id);
            if (peca != null)
            {
                _context.Pecas.Remove(peca);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<Peca>> GetAllAsync()
        {
            return await _context.Pecas.ToListAsync();
        }

        public async Task<Peca> GetByIdAsync(int id)
        {
            return await _context.Pecas.FirstOrDefaultAsync(u => u.IdPeca == id);
        }

        public async Task UpdateAsync(Peca peca)
        {
            _context.Pecas.Update(peca);
            await _context.SaveChangesAsync();
        }
    }
}
