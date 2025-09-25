using Garagem75.Data;
using Garagem75.Interfaces;
using Garagem75.Models;
using Microsoft.EntityFrameworkCore;

namespace Garagem75.Repositories
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly Garagem75DBContext _context;
        public UsuarioRepository(Garagem75DBContext context)
        {
            _context = context;
        }
        //create
        public async Task AddAsync(Usuario usuario)
        {
            await _context.Usuarios.AddAsync(usuario);
            await _context.SaveChangesAsync();
        }
        //read
        public async Task<List<Usuario>> GetAllAsync()
        {
            return await _context.Usuarios.Include(u => u.TipoUsuario).ToListAsync();
        }
        //read ativos
        public async Task<List<Usuario>> GetAllAtivosAsync()
        {
            return await _context.Usuarios.Where(u => u.Ativo).Include(u => u.TipoUsuario).ToListAsync();
        }
        //search by id 
        public async Task<Usuario> GetByIdAsync(int id)
        {
            return await _context.Usuarios
                .Include(u => u.TipoUsuario)
                .FirstOrDefaultAsync(u => u.IdUsuario == id);
        }
        // inativar usuario
        public async Task InativarAsync(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario != null && usuario.Ativo)
            {
                usuario.Ativo = false;
                await _context.SaveChangesAsync();
            }
        }

        public async Task ReativarAsync(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario != null && !usuario.Ativo)
            {
                usuario.Ativo = true;
                await _context.SaveChangesAsync();
            }
        }
        //update
        public async Task UpdateAsync(Usuario usuario)
        {
            _context.Usuarios.Update(usuario);
            await _context.SaveChangesAsync();
        }

        public async Task<Usuario?> ValidarLoginAsync(string email, string senha)
        {
            return await _context.Usuarios.Include(u => u.TipoUsuario)
                .FirstOrDefaultAsync(u => u.Email == email && u.Senha == senha);
        }
    }
}
