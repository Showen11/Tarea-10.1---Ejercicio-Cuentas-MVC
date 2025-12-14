using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Cuentas.Data;
using Cuentas.Models;

namespace Cuentas.Controllers
{
    public class MovimientosController : Controller
    {
        private readonly CuentasContext _context;

        public MovimientosController(CuentasContext context)
        {
            _context = context;
        }

        // GET: Movimientos
        public async Task<IActionResult> Index()
        {
            return View(await _context.Movimiento.ToListAsync());
        }

        // GET: Movimientos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movimiento = await _context.Movimiento
                .FirstOrDefaultAsync(m => m.Id == id);
            if (movimiento == null)
            {
                return NotFound();
            }

            return View(movimiento);
        }

        // GET: Movimientos/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Movimientos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Tipo,Concepto,Monto,Fecha,IdCuenta")] Movimiento movimiento)
        {
            if (!ModelState.IsValid)
                return View(movimiento);

            var cuenta = await _context.Cuenta
                .FirstOrDefaultAsync(c => c.Id == movimiento.IdCuenta);

            if (cuenta == null)
            {
                ModelState.AddModelError("","La cuenta especificada no existe.");
                return View(movimiento);
            }

            if (cuenta.balance <= 0)
            {
                ModelState.AddModelError("", "La cuenta esta inactiva, no se puede realizar el movimiento.");
                return View(movimiento);
            }

            if (movimiento.Tipo == 1) //D
            {
                cuenta.setCreditos(movimiento.Monto); 
            }
            else if (movimiento.Tipo == 2) //R
            {
                cuenta.setDebito(movimiento.Monto); 
            }

            cuenta.setBalance();

            _context.Add(movimiento);
            _context.Update(cuenta);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: Movimientos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movimiento = await _context.Movimiento.FindAsync(id);
            if (movimiento == null)
            {
                return NotFound();
            }
            return View(movimiento);
        }

        // POST: Movimientos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Tipo,Concepto,Monto,Fecha,IdCuenta")] Movimiento movimiento)
        {
            if (id != movimiento.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(movimiento);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MovimientoExists(movimiento.Id))
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
            return View(movimiento);
        }

        // GET: Movimientos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movimiento = await _context.Movimiento
                .FirstOrDefaultAsync(m => m.Id == id);
            if (movimiento == null)
            {
                return NotFound();
            }

            return View(movimiento);
        }

        // POST: Movimientos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var movimiento = await _context.Movimiento.FindAsync(id);
            if (movimiento != null)
            {
                _context.Movimiento.Remove(movimiento);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MovimientoExists(int id)
        {
            return _context.Movimiento.Any(e => e.Id == id);
        }
    }
}
