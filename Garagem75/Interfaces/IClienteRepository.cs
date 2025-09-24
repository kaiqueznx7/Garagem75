using Garagem75.Models;

namespace Garagem75.Interfaces
{
    public interface IClienteRepository
    {
        Task<List<Cliente>> GetAllAsync();

        //stand by
        Task<Cliente> GetByIdAsync(int id);
        Task AddAsync(Cliente cliente);
        Task UpdateAsync(Cliente cliente);
        Task DeleteAsync(int id);
    }
}
