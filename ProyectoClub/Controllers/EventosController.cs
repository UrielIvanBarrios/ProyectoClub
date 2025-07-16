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

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var evento = await _context.Eventos.FindAsync(id);
            if (evento == null) return NotFound();

            ViewBag.SedeId = new SelectList(_context.Sedes, "Id", "Nombre", evento.SedeId);
            return View(evento);
        }

        // POST: Eventos/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Evento evento)
        {
            if (id != evento.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    // Aseguramos que no se modifique el UsuarioId
                    var eventoExistente = await _context.Eventos.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id);
                    if (eventoExistente == null) return NotFound();

                    evento.UsuarioId = eventoExistente.UsuarioId;

                    _context.Update(evento);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Eventos.Any(e => e.Id == evento.Id)) return NotFound();
                    else throw;
                }

                return RedirectToAction(nameof(Index));
            }

            ViewBag.SedeId = new SelectList(_context.Sedes, "Id", "Nombre", evento.SedeId);
            return View(evento);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var evento = await _context.Eventos
                .Include(e => e.Sede)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (evento == null) return NotFound();

            return View(evento);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var evento = await _context.Eventos.FindAsync(id);
            _context.Eventos.Remove(evento);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
