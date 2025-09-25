using Garagem75.Data;
using Garagem75.Interfaces;
using Garagem75.Models;
using Microsoft.EntityFrameworkCore;

namespace Garagem75.Repositories
{
    public class ClienteRepository : IClienteRepository
    {
        private readonly Garagem75DBContext _context;
        public ClienteRepository(Garagem75DBContext context)
        {
            _context = context;
        }
        public async Task AddAsync(Cliente cliente)
        {
            await _context.Clientes.AddAsync(cliente);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var cliente = await _context.Clientes.FirstOrDefaultAsync(c => c.IdCliente == id);
            if (cliente != null)
            {
                _context.Clientes.Remove(cliente);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<Cliente>> GetAllAsync()
        {
            return await _context.Clientes.ToListAsync();
        }

        public async Task<Cliente> GetByIdAsync(int id)
        {
            return await _context.Clientes.FirstOrDefaultAsync(u => u.IdCliente == id);
        }

        public async Task UpdateAsync(Cliente cliente)
        {
            _context.Clientes.Update(cliente);
            await _context.SaveChangesAsync();
        }
    }
}
