using System.ComponentModel.DataAnnotations;

namespace Garagem75.Models;

    public class OrdemServico
    {
    [Key]
    public int IdOrdemServico { get; set; }

    [Required]
    [StringLength(150)]
    public string Descricao { get; set; }

    public DateTime DataServico { get; set; }
    [Required]
    public decimal MaoDeObra { get; set; }

    public decimal? ValorDesconto { get; set; }

    [Required]
    public decimal ValorTotal { get; set; }

    [Required]
    public string Status { get; set; }
    [Required]
    public DateTime DataEntrega { get; set; }

    public virtual Veiculo? Veiculo { get; set; }


}

