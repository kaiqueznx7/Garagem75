using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Garagem75.Models;
[Table("Peca")]
public class Peca
    {
    [Key]
    public int IdPeca { get; set; }

    [Required(ErrorMessage = "O campo Código da Peça é obrigatório.")]
    public int CodPeca { get; set; }

    [Required(ErrorMessage = "A Marca da Peça é obrigatório.")]
    [StringLength(50)]
    public string Marca { get; set; }

    [Required(ErrorMessage = "O nome da peça é obrigatório")]
    [StringLength(100)]
    public string Nome { get; set; }

    [Required]

    [Column(TypeName = "decimal(18,2)")]
    public decimal Preco { get; set; }
    [Required(ErrorMessage ="O Fornecedor é obrigatório.")]
    [StringLength(100)]
    public string Fornecedor { get; set; }

    [Required(ErrorMessage = "A Quantidade em Estoque é obrigatório.")]
    public int QuantidadeEstoque { get; set; }

    [Required]
    public DateTime DataCadastro { get; set; }

    [Required]
    public DateTime DataUltimaAtualizacao { get; set; }

}

