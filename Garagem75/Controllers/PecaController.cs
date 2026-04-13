using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Garagem75.Client.Services;
using Garagem75.Shared.Dtos;
using System.Globalization;


    [Authorize(Roles = "Administrador, Mecanico")]
    public class PecaController : Controller
    {
        private readonly PecaApiService _api;

        public PecaController(PecaApiService api)
        {
            _api = api;
        }

    // ================= INDEX =================

    public async Task<IActionResult> Index(string marca, string searchString)

    {

        var pecas = await _api.GetAll();



        var marcas = pecas

        .Select(p => p.Marca)

        .Distinct()

        .OrderBy(m => m)

        .ToList();



        ViewBag.Marcas = new SelectList(marcas);



        // Filtro Case-Insensitive (para aceitar minúsculas como você pediu antes)

        if (!string.IsNullOrEmpty(marca))

            pecas = pecas.Where(p => p.Marca == marca).ToList();



        if (!string.IsNullOrEmpty(searchString))

        {

            var busca = searchString.ToLower();

            pecas = pecas.Where(p =>

            p.Nome.ToLower().Contains(busca) ||

            p.Fornecedor.ToLower().Contains(busca)

            ).ToList();

        }



        return View(pecas);

    }

    // ================= DETAILS =================

    public async Task<IActionResult> Details(int id)

    {

        var peca = await _api.GetById(id);

        if (peca == null) return NotFound();



        return View(peca);

    }

    // ================= CREATE =================
    public IActionResult Create() => View(new PecaDto());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PecaDto dto, IFormFile? ImagemUpload)
        {
            TratarPreco(dto); // Método auxiliar criado abaixo

            if (!ModelState.IsValid) return View(dto);

            // 🚀 1. CRIA PEÇA NA API
            var criada = await _api.Create(dto);

            // 🚀 2. UPLOAD DA IMAGEM PARA A API
            if (criada != null && ImagemUpload != null)
            {
                await _api.UploadImagem(criada.IdPeca, ImagemUpload);
            }

            return RedirectToAction(nameof(Index));
        }

        // ================= EDIT =================
        public async Task<IActionResult> Edit(int id)
        {
            var peca = await _api.GetById(id);
            if (peca == null) return NotFound();
            return View(peca);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PecaDto dto, IFormFile? ImagemUpload)
        {
            if (id != dto.IdPeca) return NotFound();

            TratarPreco(dto);

            if (!ModelState.IsValid) return View(dto);

            // 🔥 1. ATUALIZA DADOS NA API
            await _api.Update(dto);

            // 🔥 2. SE TROCOU A IMAGEM, ENVIA PARA A API
            if (ImagemUpload != null && ImagemUpload.Length > 0)
            {
                await _api.UploadImagem(dto.IdPeca, ImagemUpload);
            }

            return RedirectToAction(nameof(Index));
        }

        // ================= DELETE =================
        public async Task<IActionResult> Delete(int id)
        {
            var peca = await _api.GetById(id);
            if (peca == null) return NotFound();
            return View(peca);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _api.Delete(id);
            return RedirectToAction(nameof(Index));
        }

        // Método auxiliar para não repetir código de preço
        private void TratarPreco(PecaDto dto)
        {
            if (Request.Form.TryGetValue("Preco", out var precoTexto))
            {
                try
                {
                    var valorLimpo = precoTexto.ToString()
                        .Replace("R$", "").Trim()
                        .Replace(".", "")
                        .Replace(",", ".");

                    dto.Preco = decimal.Parse(valorLimpo, CultureInfo.InvariantCulture);
                }
                catch
                {
                    ModelState.AddModelError("Preco", "Formato de preço inválido.");
                }
            }
        }

    //// Método auxiliar no final do Controller para evitar repetição
    //private string CorrigirUrlImagem(string urlOriginal, string baseUrl)
    //{
    //    if (string.IsNullOrEmpty(urlOriginal)) return "";

    //    // 1. Força a baseUrl a ser HTTPS (o Dev Tunnel exige isso)
    //    if (baseUrl.StartsWith("http://"))
    //    {
    //        baseUrl = baseUrl.Replace("http://", "https://");
    //    }

    //    // 2. Se a URL do banco contém localhost, trocamos pelo domínio do túnel
    //    if (urlOriginal.Contains("localhost"))
    //    {
    //        try
    //        {
    //            var uri = new Uri(urlOriginal);
    //            // uri.PathAndQuery pega tudo depois da porta, ex: /uploads/pecas/foto.jpg
    //            return $"{baseUrl}{uri.PathAndQuery}";
    //        }
    //        catch { return urlOriginal; }
    //    }

    //    // 3. Caso a URL já venha sem localhost mas sem HTTPS
    //    if (urlOriginal.StartsWith("http://"))
    //    {
    //        return urlOriginal.Replace("http://", "https://");
    //    }

    //    return urlOriginal;
    //}
}
