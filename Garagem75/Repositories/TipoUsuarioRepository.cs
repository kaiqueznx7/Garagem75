using Garagem75.Data;
using Garagem75.Interfaces;
using Garagem75.Models;
using Microsoft.EntityFrameworkCore;

namespace Garagem75.Repositories
{
    public class TipoUsuarioRepository : ITipoUsuarioRepository
    {
        private readonly Garagem75DBContext _context;
        public TipoUsuarioRepository(Garagem75DBContext context)
        {
            _context = context;
        }
        public async Task<List<TipoUsuario>> GetAllAsync()
        {
            return await _context.TipoUsuarios.ToListAsync();
        }
        public Task AddAsync(TipoUsuario tipoUsuario)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }


        public Task<TipoUsuario> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(TipoUsuario tipoUsuario)
        {
            throw new NotImplementedException();
        }
    }
}
