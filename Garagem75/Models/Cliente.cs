using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Garagem75.Models;

[Table("Cliente")]
public class Cliente
{
    [Key]
    public int IdCliente { get; set; }

    [Required(ErrorMessage = "Campo obrigatório!")]
    [Display(Name = "Nome")]
    [StringLength(75)]
    public string Nome { get; set; }

    [Required(ErrorMessage = "Campo obrigatório!")]
    [Display(Name = "CPF")]
    [StringLength(11)]
    public string Cpf { get; set; }

    [Required(ErrorMessage = "Campo obrigatório!")]
    [Display(Name = "Telefone")]
    [StringLength(15)]
    public string Telefone { get; set; }

    [Required(ErrorMessage = "Campo obrigatório!")]
    [Display(Name = "Email")]
    [StringLength(75)]
    public string Email { get; set; }
}


