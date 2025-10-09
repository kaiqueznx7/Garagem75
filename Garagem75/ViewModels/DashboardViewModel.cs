using Microsoft.AspNetCore.Mvc.Filters;

namespace Garagem75.ViewModels;

    public class DashboardViewModel
    {
    // cards
    public int TotalPecas { get; set; }
    public int TotalClientes { get; set; }
    public int TotalOrdensServico { get; set; }
    public int TotalUsuarios { get; set; }


    // Listas
    public List<PecaItem> UltimasPecas { get; set; } = new();
    public List<PecaItem> MarcasMaisUsadas { get; set; } = new();
    public List<ClienteCount> ClientesMaisAntigos { get; set; } = new();

    // Gráficos
    public List<ClienteCount> ClientePorOrdemServico { get; set; } = new();
    public List<OrdensServicoCount> OrdemServico { get; set; } = new();
}


// ====== TIPOS DE APOIO (no MESMO namespace) ======
public class PecaItem
{
    public int IdPeca { get; set; }
    public string Marca { get; set; } = "";
    public string Nome { get; set; } = "";
    public decimal Preco { get; set; }
   
}

public class ClienteCount
{
    public int IdCliente { get; set; }
    public string Nome { get; set; } = "";
    public string Telefone { get; set; } = "";
    public string Email { get; set; } = "";
}

public class OrdensServicoCount
{
    public int IdOrdemServico { get; set; }
    public string DataServico { get; set; } = "";
    public int ValorTotal { get; set; }
    public int QtdOrdemServico { get; set; } 
}

