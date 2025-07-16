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


        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> IndexActividad()
        {
            var user = await _userManager.GetUserAsync(User);
            var query = _context.Inscripciones
                .Include(i => i.Usuario)
                .Include(i => i.Actividad)
                .Include(i => i.Sede)
                .Where(i => i.ActividadId != null)
                .AsQueryable();

            if (!User.IsInRole("Admin"))
            {
                query = query.Where(i => i.UsuarioId == user.Id);
            }

            return View("IndexActividad", await query.ToListAsync());
        }

        public async Task<IActionResult> IndexEvento()
        {
            var user = await _userManager.GetUserAsync(User);
            var query = _context.Inscripciones
                .Include(i => i.Usuario)
                .Include(i => i.Evento)
                .Include(i => i.Sede)
                .Where(i => i.EventoId != null)
                .AsQueryable();

            if (!User.IsInRole("Admin"))
            {
                query = query.Where(i => i.UsuarioId == user.Id);
            }

            return View("IndexEvento", await query.ToListAsync());
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
            var userId = _userManager.GetUserId(User);

            ViewBag.SedeId = new SelectList(_context.Sedes, "Id", "Nombre");

            var actividadesDisponibles = _context.Actividades
                .Where(a => !_context.Inscripciones
                    .Any(i => i.UsuarioId == userId && i.ActividadId == a.Id))
                .ToList();

            ViewBag.ActividadId = new SelectList(actividadesDisponibles, "Id", "Nombre");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CrearActividad(Inscripcion inscripcion)
        {
            inscripcion.UsuarioId = _userManager.GetUserId(User);
            inscripcion.FechaInscripcion = DateTime.Now;

            if (inscripcion.ActividadId == null)
                ModelState.AddModelError("ActividadId", "Debe seleccionar una actividad.");

            bool yaInscrito = await _context.Inscripciones
                .AnyAsync(i => i.UsuarioId == inscripcion.UsuarioId &&
                      i.ActividadId == inscripcion.ActividadId &&
                      i.SedeId == inscripcion.SedeId);

            if (yaInscrito)
            {
                ModelState.AddModelError("", "Ya estás inscrito en esta actividad en la sede seleccionada.");
            }

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

        [HttpGet]
        public JsonResult ObtenerEventosPorSede(int sedeId)
        {
            var eventos = _context.Eventos
                .Where(e => e.SedeId == sedeId)
                .Select(e => new
                {
                    id = e.Id,
                    nombre = e.Nombre
                })
                .ToList();

            return Json(eventos);
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

            bool yaInscrito = await _context.Inscripciones
                .AnyAsync(i => i.UsuarioId == inscripcion.UsuarioId &&
                      i.EventoId == inscripcion.EventoId &&
                      i.SedeId == inscripcion.SedeId);

            if (yaInscrito)
            {
                ModelState.AddModelError("", "Ya estás inscrito en este evento en la sede seleccionada.");
            }

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


        public async Task<IActionResult> EditActividad(int? id)
        {
            if (id == null)
                return NotFound();

            var inscripcion = await _context.Inscripciones
                .Include(i => i.Actividad)
                .Include(i => i.Sede)
                .Include(i => i.Usuario)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (inscripcion == null || inscripcion.ActividadId == null)
                return NotFound();

            ViewBag.SedeId = new SelectList(_context.Sedes, "Id", "Nombre", inscripcion.SedeId);

            var actividades = await _context.SedesActividad
                .Where(sa => sa.SedeId == inscripcion.SedeId && sa.Actividad.Habilitada)
                .Select(sa => sa.Actividad)
                .Distinct()
                .ToListAsync();

            ViewBag.ActividadId = new SelectList(actividades, "Id", "Nombre", inscripcion.ActividadId);

            return View(inscripcion);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditActividad(int id, [Bind("Id,SedeId,ActividadId,UsuarioId,FechaInscripcion")] Inscripcion inscripcion)
        {
            if (id != inscripcion.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(inscripcion);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(IndexActividad));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Inscripciones.Any(i => i.Id == inscripcion.Id))
                        return NotFound();

                    throw;
                }
            }

            ViewBag.SedeId = new SelectList(_context.Sedes, "Id", "Nombre", inscripcion.SedeId);

            var actividades = await _context.SedesActividad
                .Where(sa => sa.SedeId == inscripcion.SedeId && sa.Actividad.Habilitada)
                .Select(sa => sa.Actividad)
                .Distinct()
                .ToListAsync();

            ViewBag.ActividadId = new SelectList(actividades, "Id", "Nombre", inscripcion.ActividadId);

            return View(inscripcion);
        }


        public async Task<IActionResult> EditEvento(int? id)
        {
            if (id == null)
                return NotFound();

            var inscripcion = await _context.Inscripciones
                .Include(i => i.Evento)
                .Include(i => i.Sede)
                .Include(i => i.Usuario)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (inscripcion == null || inscripcion.EventoId == null)
                return NotFound();

            ViewBag.SedeId = new SelectList(_context.Sedes, "Id", "Nombre", inscripcion.SedeId);
            ViewBag.EventoId = new SelectList(_context.Eventos.Where(e => e.SedeId == inscripcion.SedeId), "Id", "Nombre", inscripcion.EventoId);

            return View(inscripcion);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditEvento(int id, Inscripcion inscripcion)
        {
            if (id != inscripcion.Id)
                return NotFound();

            if (!ModelState.IsValid)
                return View(inscripcion);

            try
            {
                _context.Update(inscripcion);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(IndexEvento));
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine("ERROR DB: " + ex.InnerException?.Message);
                ModelState.AddModelError("", "Error al guardar los cambios. Intente nuevamente o contacte al administrador.");
            }

            ViewData["SedeId"] = new SelectList(_context.Sedes, "Id", "Nombre", inscripcion.SedeId);
            ViewData["EventoId"] = new SelectList(_context.Eventos, "Id", "Nombre", inscripcion.EventoId);

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
