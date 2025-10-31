using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Garagem75.Data;
using Garagem75.Models;
using System.Threading.Tasks;
using System.Linq;

namespace Garagem75.Controllers
{
    public class BlogController : Controller
    {
        private readonly Garagem75DBContext _context;

        public BlogController(Garagem75DBContext context)
        {
            _context = context;
        }

        // Lista de posts
        public async Task<IActionResult> Index()
        {
            var posts = await _context.BlogPosts
                .Where(p => p.Ativo)
                .OrderByDescending(p => p.DataPublicacao)
                .ToListAsync();

            return View(posts);
        }

        // Página de detalhes
        public async Task<IActionResult> Detalhes(int id)
        {
            var post = await _context.BlogPosts.FindAsync(id);
            if (post == null || !post.Ativo)
                return NotFound();

            return View(post);
        }
    }
}
