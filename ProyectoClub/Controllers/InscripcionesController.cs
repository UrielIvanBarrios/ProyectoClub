using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProyectoClub.Data;
using ProyectoClub.Models;

namespace ProyectoClub.Controllers
{
    [Authorize]
    public class InscripcionesController : Controller
    {
        private readonly ProyectoClubDbContext _context;
        private readonly UserManager<Usuario> _userManager;
        public InscripcionesController(ProyectoClubDbContext context, UserManager<Usuario> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            var inscripciones = await _context.Inscripciones
                .Include(i => i.Sede)
                .Include(i => i.Actividad)
                .Include(i => i.Evento)
                .Where(i => i.UsuarioId == userId)
                .ToListAsync();

            return View(inscripciones);
        }

        [HttpGet]
        public JsonResult ObtenerActividadesPorSede(int sedeId)
        {
            var actividades = _context.SedesActividad
                .Where(sa => sa.SedeId == sedeId)
                .Select(sa => new {
                    id = sa.Actividad.Id,
                    nombre = sa.Actividad.Nombre
                })
                .ToList();

            return Json(actividades);
        }

        public IActionResult CrearActividad()
        {
            ViewBag.SedeId = new SelectList(_context.Sedes, "Id", "Nombre");
            ViewBag.ActividadId = new SelectList(_context.Actividades, "Id", "Nombre");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CrearActividad(Inscripcion inscripcion)
        {
            inscripcion.UsuarioId = _userManager.GetUserId(User);
            inscripcion.FechaInscripcion = DateTime.Now;

            // Validación simple: que haya actividad
            if (inscripcion.ActividadId == null)
                ModelState.AddModelError("ActividadId", "Debe seleccionar una actividad.");

            if (ModelState.IsValid)
            {
                _context.Add(inscripcion);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.SedeId = new SelectList(_context.Sedes, "Id", "Nombre", inscripcion.SedeId);
            ViewBag.ActividadId = new SelectList(_context.Actividades, "Id", "Nombre", inscripcion.ActividadId);
            return View(inscripcion);
        }

        public IActionResult CrearEvento()
        {
            ViewBag.SedeId = new SelectList(_context.Sedes, "Id", "Nombre");
            ViewBag.EventoId = new SelectList(_context.Eventos, "Id", "Nombre");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CrearEvento(Inscripcion inscripcion)
        {
            inscripcion.UsuarioId = _userManager.GetUserId(User);
            inscripcion.FechaInscripcion = DateTime.Now;

            if (inscripcion.EventoId == null)
                ModelState.AddModelError("EventoId", "Debe seleccionar un evento.");

            if (ModelState.IsValid)
            {
                _context.Add(inscripcion);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.SedeId = new SelectList(_context.Sedes, "Id", "Nombre", inscripcion.SedeId);
            ViewBag.EventoId = new SelectList(_context.Eventos, "Id", "Nombre", inscripcion.EventoId);
            return View(inscripcion);
        }


        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var inscripcion = await _context.Inscripciones
                .Include(i => i.Sede)
                .Include(i => i.Actividad)
                .Include(i => i.Evento)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (inscripcion == null) return NotFound();

            return View(inscripcion);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var inscripcion = await _context.Inscripciones.FindAsync(id);
            _context.Inscripciones.Remove(inscripcion);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
