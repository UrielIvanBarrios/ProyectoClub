using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProyectoClub.Data;
using ProyectoClub.Models;

namespace ProyectoClub.Controllers
{
    [Authorize(Roles = "Admin")]
    public class EventosController : Controller
    {
        private readonly ProyectoClubDbContext _context;
        private readonly UserManager<Usuario> _userManager;

        public EventosController(ProyectoClubDbContext context, UserManager<Usuario> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            return View(await _context.Eventos.Include(e => e.Sede).ToListAsync());
        }

        public IActionResult Create()
        {
            ViewBag.SedeId = new SelectList(_context.Sedes, "Id", "Nombre");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Evento evento)
        {
            evento.UsuarioId = _userManager.GetUserId(User);

            if (ModelState.IsValid)
            {
                _context.Add(evento);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.SedeId = new SelectList(_context.Sedes, "Id", "Nombre", evento.SedeId);
            return View(evento);
        }
    }
}
