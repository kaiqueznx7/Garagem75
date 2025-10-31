using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Garagem75.Data;
using Garagem75.Models;
using System.Threading.Tasks;
using System.Linq;

namespace Garagem75.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class BlogAdminController : Controller
    {
        private readonly Garagem75DBContext _context;

        public BlogAdminController(Garagem75DBContext context)
        {
            _context = context;
        }

        // LISTAR POSTS
        public async Task<IActionResult> Index()
        {
            var posts = await _context.BlogPosts
                .OrderByDescending(p => p.DataPublicacao)
                .ToListAsync();
            return View(posts);
        }

        // GET: CRIAR POST
        public IActionResult Create()
        {
            return View();
        }

        // POST: CRIAR POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BlogPost post)
        {
            if (ModelState.IsValid)
            {
                post.DataPublicacao = DateTime.Now;
                _context.Add(post);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Post criado com sucesso!";
                return RedirectToAction(nameof(Index));
            }
            return View(post);
        }

        // GET: EDITAR POST
        public async Task<IActionResult> Edit(int id)
        {
            var post = await _context.BlogPosts.FindAsync(id);
            if (post == null)
                return NotFound();

            return View(post);
        }

        // POST: EDITAR POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, BlogPost post)
        {
            if (id != post.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(post);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Post atualizado com sucesso!";
                return RedirectToAction(nameof(Index));
            }
            return View(post);
        }

        // GET: EXCLUIR POST
        public async Task<IActionResult> Delete(int id)
        {
            var post = await _context.BlogPosts.FindAsync(id);
            if (post == null)
                return NotFound();

            return View(post);
        }

        // POST: CONFIRMAR EXCLUSÃO
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var post = await _context.BlogPosts.FindAsync(id);
            if (post != null)
            {
                _context.BlogPosts.Remove(post);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Post excluído com sucesso!";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
