using Garagem75.Models;
using System.ComponentModel.DataAnnotations;

namespace Garagem75.ViewModels
{
    public class VeiculoViewModel
    {
        public int IdVeiculo { get; set; }

        public string Fabricante { get; set; }
        
        public string Modelo { get; set; }

        
        public DateTime Ano { get; set; }

        
        public string Placa { get; set; }

        public string Cor { get; set; }
        public int ClienteId { get; set; }




    }
}
