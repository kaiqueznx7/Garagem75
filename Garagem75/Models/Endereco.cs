using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Garagem75.Models;

[Table("Endereco")]
public class Endereco
    {
    [Key]
    public int IdEndereco { get; set; }

    [Required(ErrorMessage ="Rua é Obrigatório")]
    [Display(Name = "Nome da Rua")]
    [StringLength(100)]
    public string Rua { get; set; }

    [Required(ErrorMessage = "Número é Obrigatório")]
    [Display(Name = "Número")]
    public int Numero { get; set; }

    [Display(Name = "Complemento")]
    [StringLength(50)]
    public string? Complemento { get; set; }

    [Required(ErrorMessage = "Bairro é Obrigatório")]
    [Display(Name = "Bairro")]
    [StringLength(50)]
    public string Bairro { get; set; }

    [Required(ErrorMessage = "Cidade é Obrigatório")]
    [Display(Name = "Cidade")]
    [StringLength(2)]
    public string Uf { get; set; }

    [Required(ErrorMessage = "CEP é Obrigatório")]
    [Display(Name = "CEP")]
    [StringLength(9)]
    public string Cep { get; set; }

    [Required(ErrorMessage = "Indique se é o Endereço Principal")]
    [Display(Name = "Endereço Principal")]
    public bool Principal { get; set; }

    public virtual Cliente? Cliente { get; set; }
}

