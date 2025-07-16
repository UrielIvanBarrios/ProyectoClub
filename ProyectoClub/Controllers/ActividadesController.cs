using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProyectoClub.Data;
using ProyectoClub.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProyectoClub.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ActividadesController : Controller
    {
        private readonly ProyectoClubDbContext _context;

        public ActividadesController(ProyectoClubDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Actividades.ToListAsync());
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nombre,Descripcion,Habilitada")] Actividad actividad)
        {
            if (_context.Actividades.Any(a => a.Nombre == actividad.Nombre))
            {
                ModelState.AddModelError("Nombre", "Ya existe una actividad con este nombre.");
            }

            if (ModelState.IsValid)
            {
                _context.Add(actividad);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(actividad);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var actividad = await _context.Actividades.FindAsync(id);
            if (actividad == null)
            {
                return NotFound();
            }
            return View(actividad);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nombre,Descripcion,Habilitada")] Actividad actividad)
        {
            if (id != actividad.Id)
            {
                return NotFound();
            }

            if (_context.Actividades.Any(a => a.Nombre == actividad.Nombre && a.Id != actividad.Id))
            {
                ModelState.AddModelError("Nombre", "Ya existe una actividad con este nombre.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(actividad);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ActividadExists(actividad.Id))
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
            return View(actividad);
        }

        // GET: Actividades/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var actividad = await _context.Actividades
                .FirstOrDefaultAsync(m => m.Id == id);
            if (actividad == null)
            {
                return NotFound();
            }

            return View(actividad);
        }

        // POST: Actividades/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var actividad = await _context.Actividades.FindAsync(id);
            if (actividad != null)
            {
                _context.Actividades.Remove(actividad);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ActividadExists(int id)
        {
            return _context.Actividades.Any(e => e.Id == id);
        }
    }
}
