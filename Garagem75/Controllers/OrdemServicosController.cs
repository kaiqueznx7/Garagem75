using Microsoft.AspNetCore.Mvc;
using Garagem75.Shared.Dtos;
using Garagem75.Client.Services; // Certifique-se de que o namespace do Service está aqui

public class OrdemServicosController : Controller
{
    private readonly OrdemServicoApiService _api;
    private readonly PecaApiService _pecaApi;

    public OrdemServicosController(OrdemServicoApiService api, PecaApiService pecaApi)
    {
        _api = api;
        _pecaApi = pecaApi;
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
    public IActionResult Create()
    {
        // Se precisar carregar Veículos ou Clientes para o Select, faça aqui
        return View(new OrdemServicoDto { DataServico = DateTime.Now });
    }

    // 🔹 CREATE (POST)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(OrdemServicoDto dto)
    {
        if (!ModelState.IsValid) return View(dto);

        var ok = await _api.Create(dto);
        if (!ok)
        {
            ModelState.AddModelError("", "Erro ao salvar a Ordem de Serviço na API.");
            return View(dto);
        }

        return RedirectToAction(nameof(Index));
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
        if (id != dto.IdOrdemServico) return NotFound();

        // Monta o DTO com as peças vindas do formulário
        dto.Pecas = new List<OrdemServicoPecaDto>();
        if (pecaIds != null && quantidades != null)
        {
            for (int i = 0; i < pecaIds.Length; i++)
            {
                if (quantidades[i] > 0)
                {
                    dto.Pecas.Add(new OrdemServicoPecaDto { PecaId = pecaIds[i], Quantidade = quantidades[i] });
                }
            }
        }


        if (!ModelState.IsValid)
        {
            // 🔥 Recarrega a ViewBag antes de voltar para a tela
            await CarregarPecasParaViewBag();
            return View(dto);
        }

        var ok = await _api.Update(id, dto);

        if (!ok)
        {
            ModelState.AddModelError("", "Erro ao atualizar a Ordem de Serviço na API.");
            // 🔥 Recarrega a ViewBag aqui também
            await CarregarPecasParaViewBag();
            return View(dto);
        }

        return RedirectToAction(nameof(Index));
    }

    // 🔹 DELETE (Recomendado usar uma tela de confirmação antes)
    public async Task<IActionResult> Delete(int id)
    {
        var ok = await _api.Delete(id);
        return RedirectToAction(nameof(Index));
    }

    private async Task CarregarPecasParaViewBag()
    {
        // Substitua '_pecaApi.GetAll()' pela chamada real que você usa para listar peças
        var todasAsPecas = await _pecaApi.GetAll();
        ViewBag.Pecas = todasAsPecas;
    }
}