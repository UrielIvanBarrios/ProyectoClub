using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProyectoClub.Data;
using ProyectoClub.Models;


namespace ProyectoClub.Controllers
{
    [Authorize(Roles = "Admin")]
    public class SedesActividadesController : Controller
    {
        private readonly ProyectoClubDbContext _context;

        public SedesActividadesController(ProyectoClubDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var asociaciones = await _context.SedesActividad
                .Include(sa => sa.Sede)
                .Include(sa => sa.Actividad)
                .ToListAsync();

            return View(asociaciones);
        }

        public IActionResult Create()
        {
            ViewBag.SedeId = new SelectList(_context.Sedes, "Id", "Nombre");
            ViewBag.ActividadId = new SelectList(
                _context.Actividades.Where(a => a.Habilitada), "Id", "Nombre"
            );
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SedeActividad sedeActividad)
        {
            if (ModelState.IsValid)
            {
                // Validar que no exista ya la relación
                var existe = await _context.SedesActividad
                    .AnyAsync(sa => sa.SedeId == sedeActividad.SedeId && sa.ActividadId == sedeActividad.ActividadId);

                if (existe)
                {
                    ModelState.AddModelError("", "Esta actividad ya está asociada a la sede seleccionada.");
                }
                else
                {
                    _context.SedesActividad.Add(sedeActividad);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }

            ViewBag.SedeId = new SelectList(_context.Sedes, "Id", "Nombre", sedeActividad.SedeId);
            ViewBag.ActividadId = new SelectList(_context.Actividades, "Id", "Nombre", sedeActividad.ActividadId);
            return View(sedeActividad);
        }


    }
}
