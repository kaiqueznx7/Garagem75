using Garagem75.Data;
using Garagem75.Interfaces;
using Garagem75.Models;
using Microsoft.EntityFrameworkCore;

namespace Garagem75.Repositories
{
    public class EnderecoRepository : IEnderecoRepository
    {
        private readonly Garagem75DBContext _context;
        public EnderecoRepository(Garagem75DBContext context)
        {
            _context = context;
        }
        public async Task AddAsync(Endereco endereco)
        {
            await _context.Enderecos.AddAsync(endereco);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var endereco = await _context.Enderecos.FirstOrDefaultAsync(c => c.IdEndereco == id);
            if (endereco != null)
            {
                _context.Enderecos.Remove(endereco);
                await _context.SaveChangesAsync();
            }
        }
        

        public async Task<List<Endereco>> GetAllAsync()
        {
            return await _context.Enderecos.ToListAsync();
        }

        public async Task<Endereco> GetByIdAsync(int id)
        {
            return _context.Enderecos.FirstOrDefault(u => u.IdEndereco == id);
        }

        public async Task UpdateAsync(Endereco endereco)
        {
            _context.Enderecos.Update(endereco);
            await _context.SaveChangesAsync();
        }
    }
}
