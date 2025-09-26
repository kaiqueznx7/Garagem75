using Garagem75.Models;
using System.ComponentModel.DataAnnotations;

namespace Garagem75.ViewModels
{
    public class UsuarioViewModel
    {
        public int IdUsuario { get; set; }

        
        public string Nome { get; set; }

       
        public string Email { get; set; }

        
        public string Senha { get; set; }

        public int TipoUsuarioId { get; set; }
        public virtual TipoUsuario? TipoUsuario { get; set; }

        //Propriedade para Softdelete
        
        public bool Ativo { get; set; } = true;
    }
}
