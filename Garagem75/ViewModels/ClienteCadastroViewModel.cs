using Garagem75.Models;

namespace Garagem75.ViewModels
{
    public class ClienteCadastroViewModel
    {
        public Cliente Cliente { get; set; } = new Cliente();
        public Endereco Endereco { get; set; } = new Endereco();
        public Veiculo Veiculo { get; set; } = new Veiculo();
    }
}
