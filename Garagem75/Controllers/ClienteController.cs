using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Garagem75.Client.Services;
using Garagem75.Shared.Dtos;

namespace Garagem75.Controllers
{
    [Authorize(Roles = "Administrador, Mêcanico")]
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
                clientes = clientes.Where(c => c.Nome.Contains(searchNome)).ToList();

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

                // ✅ ISSO VAI FUNCIONAR
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

                // ❌ NÃO coloca Veiculo aqui (API não suporta)
            };

            await _api.Create(clienteDto);

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ClienteDto dto)
        {
            if (id != dto.Id)
                return NotFound();

            if (!ModelState.IsValid)
                return View(dto);

            await _api.Update(dto);

            return RedirectToAction(nameof(Index));
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
}