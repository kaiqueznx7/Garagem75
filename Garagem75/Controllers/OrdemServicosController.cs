using Garagem75.Client.Services; // Certifique-se de que o namespace do Service está aqui
using Garagem75.Shared.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;


[Authorize(Roles = "Administrador, Mecânico")]

public class OrdemServicosController : Controller
{
    private readonly OrdemServicoApiService _api;
    private readonly PecaApiService _pecaApi;
    private readonly VeiculoApiService _veiculosApi;

    public OrdemServicosController(OrdemServicoApiService api, PecaApiService pecaApi, VeiculoApiService veiculoApi)
    {
        _api = api;
        _pecaApi = pecaApi;
        _veiculosApi = veiculoApi;
    }

    // 🔹 LISTAGEM
    public async Task<IActionResult> Index(string search)
    {
        var lista = await _api.GetAll(search);
        return View(lista);
    }

    // 🔹 DETALHES
    public async Task<IActionResult> Details(int id)
    {
        var item = await _api.GetById(id);
        if (item == null) return NotFound();

        return View(item);
    }

    // 🔹 CREATE (TELA)
    [HttpGet]
    public async Task<IActionResult> Create()
    {
        // 1. Carrega as peças para a tabela aparecer na View
        // Supondo que você tenha o serviço PecasApi injetado
        var listaPecas = await _pecaApi.GetAll();
        ViewBag.Pecas = listaPecas;

        // 2. Carrega os veículos para o <select>
        var veiculos = await _veiculosApi.GetAll();
        ViewBag.VeiculoId = new SelectList(veiculos, "IdVeiculo", "Modelo");

        return View(new OrdemServicoDto { DataServico = DateTime.Now });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(OrdemServicoDto dto, int[] pecaIds, int[] quantidades, string MaoDeObraTexto, string ValorDescontoTexto)
    {
        // 1. CONVERSÃO MANUAL (Isso garante o valor correto 222,22)
        try
        {
            if (!string.IsNullOrEmpty(MaoDeObraTexto))
            {
                var valorLimpo = MaoDeObraTexto.Replace(".", "").Replace(",", ".");
                dto.MaoDeObra = decimal.Parse(valorLimpo, System.Globalization.CultureInfo.InvariantCulture);
            }
            if (!string.IsNullOrEmpty(ValorDescontoTexto))
            {
                var valorLimpo = ValorDescontoTexto.Replace(".", "").Replace(",", ".");
                dto.ValorDesconto = decimal.Parse(valorLimpo, System.Globalization.CultureInfo.InvariantCulture);
            }
        }
        catch { /* valor padrão 0 se der erro na conversão */ }

        // 2. LIMPEZA TOTAL DE ERROS (Força o MVC a ignorar validações automáticas chatas)
        ModelState.Clear();

        // 3. BUSCA O CLIENTE (Obrigatório para a API salvar)
        var veiculo = await _veiculosApi.GetById(dto.VeiculoId);
        if (veiculo != null) dto.ClienteId = veiculo.ClienteId;

        // 4. MONTA AS PEÇAS
        dto.PecasAssociadas = new List<OrdemServicoPecaDto>();
        if (pecaIds != null && quantidades != null)
        {
            for (int i = 0; i < pecaIds.Length; i++)
            {
                if (quantidades[i] > 0)
                {
                    dto.PecasAssociadas.Add(new OrdemServicoPecaDto { PecaId = pecaIds[i], Quantidade = quantidades[i] });
                }
            }
        }

        // 5. ENVIA PARA API
        var ok = await _api.Create(dto);
        if (ok)
        {
            return RedirectToAction(nameof(Index));
        }

        // Se chegou aqui, a API deu erro (BadRequest). Vamos recarregar a tela.
        ViewBag.Pecas = await _pecaApi.GetAll();
        ViewBag.VeiculoId = new SelectList(await _veiculosApi.GetAll(), "IdVeiculo", "Modelo", dto.VeiculoId);

        // Adiciona uma mensagem de erro para você ver na tela o que houve
        TempData["Erro"] = "A API recusou o salvamento. Verifique se todos os campos estão preenchidos.";

        return View(dto);
    }
    // 🔹 EDIT (TELA)
    public async Task<IActionResult> Edit(int id)
    {
        var item = await _api.GetById(id);
        if (item == null) return NotFound();

        // 🔥 Carrega as peças para a tabela aparecer
        await CarregarPecasParaViewBag();

        return View(item);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, OrdemServicoDto dto, int[] pecaIds, int[] quantidades)
    {
        // 1. O código do seu formulário vai preencher pecaIds e quantidades
        // 2. Você precisa converter esses arrays para a lista dentro do DTO:
        dto.PecasAssociadas = new List<OrdemServicoPecaDto>();

        if (pecaIds != null && quantidades != null)
        {
            for (int i = 0; i < pecaIds.Length; i++)
            {
                dto.PecasAssociadas.Add(new OrdemServicoPecaDto
                {
                    PecaId = pecaIds[i],
                    Quantidade = quantidades[i]
                });
            }
        }

        // 3. Agora envia o DTO completo para a API
        var sucesso = await _api.Update(id, dto);

        if (sucesso) return RedirectToAction(nameof(Index));

        // Se der erro, recarregue a ViewBag.Pecas para a tabela não sumir
        await CarregarPecasParaViewBag();
        return View(dto);
    }

    // 🔹 DELETE (Recomendado usar uma tela de confirmação antes)
    // 1. O MÉTODO QUE ABRE A PÁGINA DE CONFIRMAÇÃO (O que você quer)
    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        // Busca os dados da OS na API para exibir na tela de confirmação
        var os = await _api.GetById(id);
        if (os == null) return NotFound();

        return View(os); // Isso vai abrir a sua View com os detalhes da OS
    }

    // 2. O MÉTODO QUE REALMENTE APAGA (Chamado pelo formulário da View)
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int IdOrdemServico)
    {
        // Aqui sim você chama a API para deletar de vez
        var sucesso = await _api.Delete(IdOrdemServico);

        if (sucesso)
        {
            return RedirectToAction(nameof(Index));
        }

        return BadRequest("Erro ao excluir a Ordem de Serviço.");
    }

    private async Task CarregarPecasParaViewBag()
    {
        // Substitua '_pecaApi.GetAll()' pela chamada real que você usa para listar peças
        var todasAsPecas = await _pecaApi.GetAll();
        ViewBag.Pecas = todasAsPecas;
    }

    public async Task<IActionResult> Relatorio(int id)
    {
        // Busca os dados da OS na API
        var os = await _api.GetById(id);

        if (os == null) return NotFound();

        return View(os);
    }
}