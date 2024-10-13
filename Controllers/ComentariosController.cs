using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Proyect_1.Models;
using Proyect_1.ContextBD;

namespace Proyect_1.Controllers
{
    public class ComentariosController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ComentariosController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // POST: Comentarios/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Contenido,PublicacionId")] Comentario comentario)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                comentario.UsuarioId = user.Id;

                _context.Add(comentario);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", "Publicaciones", new { id = comentario.PublicacionId });
            }
            return View(comentario);
        }

        // GET: Comentarios/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var comentario = await _context.Comentarios
                .Include(c => c.Publicacion)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (comentario == null)
            {
                return NotFound();
            }

            return View(comentario);
        }

        // POST: Comentarios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var comentario = await _context.Comentarios.FindAsync(id);
            _context.Comentarios.Remove(comentario);
            await _context.SaveChangesAsync();
            return RedirectToAction("Details", "Publicaciones", new { id = comentario.PublicacionId });
        }
    }
}
