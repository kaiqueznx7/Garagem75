using Garagem75.Models;

namespace Garagem75.Interfaces
{
    public interface IOrdemServicoRepository
    {
        Task<List<OrdemServico>> GetAllAsync();

        //stand by
        Task<OrdemServico> GetByIdAsync(int id);
        Task AddAsync(OrdemServico ordemServico);
        Task UpdateAsync(OrdemServico ordemServico);
        Task DeleteAsync(int id);
    }
}
