using Garagem75.Shared.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("OrdemServicos")]
public class OrdemServico
{
    [Key]
    public int IdOrdemServico { get; set; }

    [Required]
    public string Descricao { get; set; }

    public DateTime DataServico { get; set; } = DateTime.Now;

    public decimal MaoDeObra { get; set; }
    public decimal ValorDesconto { get; set; }
    public decimal ValorTotal { get; set; }

    public string Status { get; set; } = "Aberta";

    public DateTime DataEntrega { get; set; }

    // 🔥 RELAÇÃO COM VEÍCULO (O veículo já possui o Cliente)
    public int VeiculoId { get; set; }

    [ForeignKey("VeiculoId")]
    public Veiculo? Veiculo { get; set; }

    public ICollection<OrdemServicoPeca> PecasAssociadas { get; set; }
        = new List<OrdemServicoPeca>();
}