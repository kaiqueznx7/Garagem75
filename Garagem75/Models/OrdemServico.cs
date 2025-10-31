using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
    [Column(TypeName = "decimal(18,2)")]
    [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = true)]
    public decimal MaoDeObra { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = true)]
    public decimal ValorDesconto { get; set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal ValorTotal { get; set; }

    [Required]
    public string Status { get; set; }
    [Required]
    public DateTime DataEntrega { get; set; }

    // relação muitos-para-muitos com Peca
    public ICollection<OrdemServicoPeca> PecasAssociadas { get; set; } = new List<OrdemServicoPeca>();

    [Column("VeiculoIdVeiculo")]
    public int VeiculoId { get; set; }

    [ForeignKey("VeiculoId")]
    public virtual Veiculo? Veiculo { get; set; }




}

