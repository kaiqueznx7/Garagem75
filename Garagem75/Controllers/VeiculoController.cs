using Garagem75.Models;
using Garagem75.Shared.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

[Authorize(Roles = "Administrador, Mecanico")]
public class VeiculoController : Controller
{
    private readonly VeiculoApiService _api;
    private readonly ClienteApiService _clienteApi;

    public VeiculoController(VeiculoApiService api, ClienteApiService clienteApi)
    {
        _api = api;
        _clienteApi = clienteApi;
    }

    // 🔹 INDEX
    public async Task<IActionResult> Index(string searchPlaca, string searchCliente)
    {
        var veiculos = await _api.GetAll(searchPlaca, searchCliente);
        return View(veiculos);
    }

    // 🔹 DETAILS
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();

        var veiculo = await _api.GetById(id.Value);

        if (veiculo == null) return NotFound();

        return View(veiculo);
    }

    // 🔹 CREATE GET
    public async Task<IActionResult> Create()
    {
        // ⚠️ Aqui você precisa de API de cliente
        var clientes = await _clienteApi.GetAll();

        ViewData["ClienteId"] = new SelectList(clientes, "Id", "Nome");

        return View();
    }

    // 🔹 CREATE POST
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(VeiculoDto veiculo)
    {
        if (!ModelState.IsValid)
        {
            var clientes = await _clienteApi.GetAll();
            ViewData["ClienteId"] = new SelectList(clientes, "Id", "Nome", veiculo.ClienteId);
            return View(veiculo);
        }

        var response = await _api.Create(veiculo);

        if (!response.IsSuccessStatusCode)
        {
            var conteudo = await response.Content.ReadAsStringAsync();
            try
            {
                var erro = System.Text.Json.JsonSerializer.Deserialize<ErroDto>(conteudo,
                    new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (erro?.Mensagem?.Contains("Placa") == true)
                    ModelState.AddModelError("Placa", erro.Mensagem);
                else
                    ModelState.AddModelError("", erro?.Mensagem ?? "Erro ao salvar.");
            }
            catch
            {
                ModelState.AddModelError("", conteudo);
            }

            var clientes = await _clienteApi.GetAll();
            ViewData["ClienteId"] = new SelectList(clientes, "Id", "Nome", veiculo.ClienteId);
            return View(veiculo);
        }

        return RedirectToAction(nameof(Index));
    }

    // 🔹 EDIT GET
    [HttpGet]
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var veiculo = await _api.GetById(id.Value);
        if (veiculo == null) return NotFound();

        var clientes = await _clienteApi.GetAll();

        ViewData["ClienteId"] = new SelectList(clientes, "Id", "Nome", veiculo.ClienteId);

        return View(veiculo);
    }

    // 🔹 EDIT POST
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, VeiculoDto veiculo)
    {
        if (id != veiculo.IdVeiculo)
            return NotFound();

        if (!ModelState.IsValid)
        {
            var clientes = await _clienteApi.GetAll();
            ViewData["ClienteId"] = new SelectList(clientes, "Id", "Nome", veiculo.ClienteId);
            return View(veiculo);
        }

        var response = await _api.Update(id, veiculo);

        if (!response.IsSuccessStatusCode)
        {
            var conteudo = await response.Content.ReadAsStringAsync();
            try
            {
                var erro = System.Text.Json.JsonSerializer.Deserialize<ErroDto>(conteudo,
                    new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (erro?.Mensagem?.Contains("Placa") == true)
                    ModelState.AddModelError("Placa", erro.Mensagem);
                else
                    ModelState.AddModelError("", erro?.Mensagem ?? "Erro ao salvar.");
            }
            catch
            {
                ModelState.AddModelError("", conteudo);
            }

            var clientes = await _clienteApi.GetAll();
            ViewData["ClienteId"] = new SelectList(clientes, "Id", "Nome", veiculo.ClienteId);
            return View(veiculo);
        }

        return RedirectToAction(nameof(Index));
    }

    private class ErroDto
    {
        public string Mensagem { get; set; }
    }

    // 🔹 DELETE GET
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();

        var veiculo = await _api.GetById(id.Value);

        if (veiculo == null) return NotFound();

        return View(veiculo);
    }

    // 🔹 DELETE POST
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await _api.Delete(id);
        return RedirectToAction(nameof(Index));
    }
}