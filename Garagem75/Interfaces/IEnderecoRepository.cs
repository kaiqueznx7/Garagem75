using Garagem75.Models;

namespace Garagem75.Interfaces
{
    public interface IEnderecoRepository
    {
        Task<List<Endereco>> GetAllAsync();

        //stand by
        Task<Endereco> GetByIdAsync(int id);
        Task AddAsync(Endereco endereco);
        Task UpdateAsync(Endereco endereco);
        Task DeleteAsync(int id);
    }
}
