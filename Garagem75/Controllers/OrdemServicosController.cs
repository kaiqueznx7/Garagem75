using Microsoft.AspNetCore.Mvc;
using Garagem75.Shared.Dtos;

public class OrdemServicosController : Controller
{
    private readonly OrdemServicoApiService _api;

    public OrdemServicosController(OrdemServicoApiService api)
    {
        _api = api;
    }

    // 🔹 LISTA
    public async Task<IActionResult> Index(string search)
    {
        var lista = await _api.GetAll(search);
        return View(lista);
    }

    // 🔹 DETALHES
    public async Task<IActionResult> Details(int id)
    {
        var item = await _api.GetById(id);

        if (item == null)
            return NotFound();

        return View(item);
    }

    // 🔹 CREATE (TELA)
    public IActionResult Create()
    {
        return View();
    }

    // 🔹 CREATE (POST)
    [HttpPost]
    public async Task<IActionResult> Create(OrdemServicoDto dto)
    {
        if (!ModelState.IsValid)
            return View(dto);

        var ok = await _api.Create(dto);

        if (!ok)
        {
            ModelState.AddModelError("", "Erro ao salvar");
            return View(dto);
        }

        return RedirectToAction(nameof(Index));
    }

    // 🔹 EDIT (TELA)
    public async Task<IActionResult> Edit(int id)
    {
        var item = await _api.GetById(id);

        if (item == null)
            return NotFound();

        return View(item);
    }

    // 🔹 EDIT (POST)
    [HttpPost]
    public async Task<IActionResult> Edit(int id, OrdemServicoDto dto)
    {
        if (id != dto.IdOrdemServico)
            return NotFound();

        if (!ModelState.IsValid)
            return View(dto);

        var ok = await _api.Update(id, dto);

        if (!ok)
        {
            ModelState.AddModelError("", "Erro ao atualizar");
            return View(dto);
        }

        return RedirectToAction(nameof(Index));
    }

    // 🔹 DELETE
    public async Task<IActionResult> Delete(int id)
    {
        await _api.Delete(id);
        return RedirectToAction(nameof(Index));
    }
}