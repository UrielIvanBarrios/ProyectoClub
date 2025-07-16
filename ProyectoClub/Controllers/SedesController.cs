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
    public class SedesController : Controller
    {
        private readonly ProyectoClubDbContext _context;

        public SedesController(ProyectoClubDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Sedes.ToListAsync());
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nombre,Direccion,capacidad,Habilitada")] Sede sede)
        {
            if (ModelState.IsValid)
            {
                _context.Add(sede);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(sede);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sede = await _context.Sedes.FindAsync(id);
            if (sede == null)
            {
                return NotFound();
            }
            return View(sede);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nombre,Direccion,capacidad,Habilitada")] Sede sede)
        {
            if (id != sede.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(sede);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SedeExists(sede.Id))
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
            return View(sede);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sede = await _context.Sedes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (sede == null)
            {
                return NotFound();
            }

            return View(sede);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var sede = await _context.Sedes.FindAsync(id);
            if (sede != null)
            {
                _context.Sedes.Remove(sede);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SedeExists(int id)
        {
            return _context.Sedes.Any(e => e.Id == id);
        }
    }
}
