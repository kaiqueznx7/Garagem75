using Garagem75.Client.Services;
using Garagem75.Shared.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static System.Runtime.InteropServices.JavaScript.JSType;


    [Authorize(Roles = "Administrador, Mecânico")]
    public class ClienteController : Controller
    {
        private readonly ClienteApiService _api;

        public ClienteController(ClienteApiService api)
        {
            _api = api;
        }

        // ================= LISTAGEM =================
        public async Task<IActionResult> Index(string searchNome, string searchTelefone)
        {
            var clientes = await _api.GetAll();

            if (!string.IsNullOrEmpty(searchNome))
                clientes = clientes.Where(c => c.Nome.ToLower().Contains(searchNome)).ToList();

            if (!string.IsNullOrEmpty(searchTelefone))
                clientes = clientes.Where(c => c.Telefone.Contains(searchTelefone)).ToList();

            return View(clientes);
        }

        // ================= CREATE =================
        [HttpGet]
        public IActionResult Create()
        {
            return View(new ClienteCadastroDto());
        }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ClienteCadastroDto model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var clienteDto = new ClienteDto
        {
            Nome = model.Cliente.Nome,
            Cpf = model.Cliente.Cpf,
            Telefone = model.Cliente.Telefone,
            Email = model.Cliente.Email,
            Enderecos = new List<EnderecoDto>
        {
            new EnderecoDto
            {
                Rua = model.Endereco.Rua,
                Numero = model.Endereco.Numero,
                Complemento = model.Endereco.Complemento,
                Bairro = model.Endereco.Bairro,
                Cidade = model.Endereco.Cidade,
                Uf = model.Endereco.Uf,
                Cep = model.Endereco.Cep
            }
        }
        };

        var response = await _api.Create(clienteDto);

        if (!response.IsSuccessStatusCode)
        {
            var conteudo = await response.Content.ReadAsStringAsync();
            try
            {
                var erro = System.Text.Json.JsonSerializer.Deserialize<ErroDto>(conteudo,
                    new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                // 👇 Aparece no campo correto da view
                if (erro?.Mensagem?.Contains("CPF") == true)
                    ModelState.AddModelError("Cliente.Cpf", erro.Mensagem);
                else if (erro?.Mensagem?.Contains("mail") == true)
                    ModelState.AddModelError("Cliente.Email", erro.Mensagem);
                else
                    ModelState.AddModelError("", erro?.Mensagem ?? "Erro ao salvar.");
            }
            catch
            {
                ModelState.AddModelError("", conteudo);
            }

            return View(model);
        }

        return RedirectToAction(nameof(Index));
    }
    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        // 1. Busca os dados na API
        var response = await _api.GetById(id);

        // 2. Se a API retornar erro ou não encontrar o cliente
        if (response == null)
        {
            return NotFound();
        }

        // 3. Passa o DTO para a View (isso preenche os campos do formulário)
        return View(response);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(ClienteDto model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var response = await _api.Update(model);

        if (!response.IsSuccessStatusCode)
        {
            var conteudo = await response.Content.ReadAsStringAsync();
            try
            {
                var erro = System.Text.Json.JsonSerializer.Deserialize<ErroDto>(conteudo,
                    new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (erro?.Mensagem?.Contains("CPF") == true)
                    ModelState.AddModelError("Cpf", erro.Mensagem);
                else if (erro?.Mensagem?.Contains("mail") == true)
                    ModelState.AddModelError("Email", erro.Mensagem);
                else
                    ModelState.AddModelError("", erro?.Mensagem ?? "Erro ao salvar.");
            }
            catch
            {
                ModelState.AddModelError("", conteudo);
            }
            // 👇 garante que os endereços não vêm null e a view não quebra
            if (model.Enderecos == null)
            {
                var clienteAtual = await _api.GetById(model.Id);
                model.Enderecos = clienteAtual?.Enderecos ?? new List<EnderecoDto>();
            }

            return View(model);
        }

        return RedirectToAction(nameof(Index));
    }

    private class ErroDto
    {
        public string Mensagem { get; set; }
    }

    // ================= DETAILS =================
    public async Task<IActionResult> Details(int id)
        {
            var cliente = await _api.GetById(id);

            if (cliente == null)
                return NotFound();

            return View(cliente);
        }

        // ================= DELETE =================
        public async Task<IActionResult> Delete(int id)
        {
            var cliente = await _api.GetById(id);

            if (cliente == null)
                return NotFound();

            return View(cliente);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _api.Delete(id);
            return RedirectToAction(nameof(Index));
        }
    }
