using Garagem75.Models;

namespace Garagem75.Interfaces;

    public interface IVeiculoRepository
    {
    Task<List<Veiculo>> GetAllAsync();

    //stand by
    Task<Veiculo> GetByIdAsync(int id);
    Task AddAsync(Veiculo veiculo);
    Task UpdateAsync(Veiculo veiculo);
    Task DeleteAsync(int id);



}

