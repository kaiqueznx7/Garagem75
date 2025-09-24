using Garagem75.Models;

namespace Garagem75.Interfaces
{
    public interface IPecaRepository
    {
        Task<List<Peca>> GetAllAsync();

        //stand by
        Task<Peca> GetByIdAsync(int id);
        Task AddAsync(Peca peca);
        Task UpdateAsync(Peca peca);
        Task DeleteAsync(int id);
    }
}
