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

            var superpuesto = await _context.Eventos.AnyAsync(e =>
                e.SedeId == evento.SedeId &&
               (
                   (evento.FechaInicio >= e.FechaInicio && evento.FechaInicio < e.FechaFin) ||
                   (evento.FechaFin > e.FechaInicio && evento.FechaFin <= e.FechaFin) ||
                   (evento.FechaInicio <= e.FechaInicio && evento.FechaFin >= e.FechaFin)
                )
            );

            if (superpuesto)
            {
                ModelState.AddModelError("FechaInicio", "Ya existe un evento en la sede seleccionada durante ese horario.");
            }

            if (evento.FechaInicio >= evento.FechaFin)
            {
                ModelState.AddModelError("FechaInicio", "La fecha de inicio debe ser anterior a la fecha de fin.");
            }

            if (evento.FechaInicio < DateTime.Now)
            {
                ModelState.AddModelError("FechaInicio", "No se puede crear un evento con fecha de inicio en el pasado.");
            }

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

            var superpuesto = await _context.Eventos.AnyAsync(e =>
                e.Id != evento.Id &&
                e.SedeId == evento.SedeId &&
               (
                   (evento.FechaInicio >= e.FechaInicio && evento.FechaInicio < e.FechaFin) ||
                   (evento.FechaFin > e.FechaInicio && evento.FechaFin <= e.FechaFin) ||
                   (evento.FechaInicio <= e.FechaInicio && evento.FechaFin >= e.FechaFin)
                )
            );

            if (superpuesto)
            {
                ModelState.AddModelError("FechaInicio", "Ya existe un evento en la sede seleccionada durante ese horario.");
            }

            if (evento.FechaInicio >= evento.FechaFin)
            {
                ModelState.AddModelError("FechaInicio", "La fecha de inicio debe ser anterior a la fecha de fin.");
            }

            if (evento.FechaInicio < DateTime.Now)
            {
                ModelState.AddModelError("FechaInicio", "No se puede crear un evento con fecha de inicio en el pasado.");
            }

            if (ModelState.IsValid)
            {
                try
                {
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
