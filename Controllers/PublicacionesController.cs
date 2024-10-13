using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Proyect_1.Models;
using Proyect_1.ContextBD;

namespace Proyect_1.Controllers
{
    public class PublicacionesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public PublicacionesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Publicaciones
        public async Task<IActionResult> Index()
        {
            var publicaciones = await _context.Publicaciones.Include(p => p.Usuario).ToListAsync();
            return View(publicaciones);
        }

        // GET: Publicaciones/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var publicacion = await _context.Publicaciones
                .Include(p => p.Usuario)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (publicacion == null)
            {
                return NotFound();
            }

            return View(publicacion);
        }

        // GET: Publicaciones/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Publicaciones/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Titulo,Contenido")] Publicacion publicacion)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                publicacion.UsuarioId = user.Id;
                _context.Add(publicacion);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(publicacion);
        }

        // GET: Publicaciones/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var publicacion = await _context.Publicaciones.FindAsync(id);
            if (publicacion == null)
            {
                return NotFound();
            }
            return View(publicacion);
        }

        // POST: Publicaciones/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Titulo,Contenido")] Publicacion publicacion)
        {
            if (id != publicacion.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(publicacion);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PublicacionExists(publicacion.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(publicacion);
        }

        // GET: Publicaciones/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var publicacion = await _context.Publicaciones
                .Include(p => p.Usuario)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (publicacion == null)
            {
                return NotFound();
            }

            return View(publicacion);
        }

        // POST: Publicaciones/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var publicacion = await _context.Publicaciones.FindAsync(id);
            _context.Publicaciones.Remove(publicacion);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PublicacionExists(int id)
        {
            return _context.Publicaciones.Any(e => e.Id == id);
        }
    }
}
