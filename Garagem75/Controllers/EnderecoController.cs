using Garagem75.Client.Services;
using Garagem75.Shared.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


    [Authorize(Roles = "Administrador, Mecanico")]
    public class EnderecoController : Controller
    {
        private readonly EnderecoApiService _api;

        public EnderecoController(EnderecoApiService api)
        {
            _api = api;
        }

        // GET: Endereco
        public async Task<IActionResult> Index()
        {
            // Busca a lista da API
            var enderecos = await _api.GetAll();
            return View(enderecos);
        }

        // GET: Endereco/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var endereco = await _api.GetById(id);
            if (endereco == null)
            {
                return NotFound();
            }

            return View(endereco);
        }

        // GET: Endereco/Create
        public IActionResult Create()
        {
            return View(new EnderecoDto());
        }

        // POST: Endereco/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EnderecoDto endereco)
        {
            if (ModelState.IsValid)
            {
                await _api.Create(endereco);
                return RedirectToAction(nameof(Index));
            }
            return View(endereco);
        }

        // GET: Endereco/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var endereco = await _api.GetById(id);
            if (endereco == null)
            {
                return NotFound();
            }
            return View(endereco);
        }

        // POST: Endereco/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EnderecoDto endereco)
        {
            // Garante que o ID da URL é o mesmo do objeto
            if (id != endereco.IdEndereco)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                await _api.Update(endereco);
                return RedirectToAction(nameof(Index));
            }
            return View(endereco);
        }

        // GET: Endereco/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var endereco = await _api.GetById(id);
            if (endereco == null)
            {
                return NotFound();
            }

            return View(endereco);
        }

        // POST: Endereco/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _api.Delete(id);
            return RedirectToAction(nameof(Index));
        }
    }
