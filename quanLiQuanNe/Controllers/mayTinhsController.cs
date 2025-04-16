using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using quanLiQuanNe.Data;
using quanLiQuanNe.Models;

namespace quanLiQuanNe.Controllers
{
    public class mayTinhsController : Controller
    {
        private readonly quanLiQuanNeContext _context;

        public mayTinhsController(quanLiQuanNeContext context)
        {
            _context = context;
        }

        // GET: mayTinhs
        public async Task<IActionResult> Index()
        {
            return View(await _context.mayTinh.ToListAsync());
        }

        // GET: mayTinhs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mayTinh = await _context.mayTinh
                .FirstOrDefaultAsync(m => m.id == id);
            if (mayTinh == null)
            {
                return NotFound();
            }

            return View(mayTinh);
        }

        // GET: mayTinhs/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: mayTinhs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,name,trangThai,donGia")] mayTinh mayTinh)
        {
            if (ModelState.IsValid)
            {
                _context.Add(mayTinh);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(mayTinh);
        }

        // GET: mayTinhs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mayTinh = await _context.mayTinh.FindAsync(id);
            if (mayTinh == null)
            {
                return NotFound();
            }
            return View(mayTinh);
        }

        // POST: mayTinhs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,name,trangThai,donGia")] mayTinh mayTinh)
        {
            if (id != mayTinh.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(mayTinh);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!mayTinhExists(mayTinh.id))
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
            return View(mayTinh);
        }

        // GET: mayTinhs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mayTinh = await _context.mayTinh
                .FirstOrDefaultAsync(m => m.id == id);
            if (mayTinh == null)
            {
                return NotFound();
            }

            return View(mayTinh);
        }

        // POST: mayTinhs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var mayTinh = await _context.mayTinh.FindAsync(id);
            if (mayTinh != null)
            {
                _context.mayTinh.Remove(mayTinh);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool mayTinhExists(int id)
        {
            return _context.mayTinh.Any(e => e.id == id);
        }
    }
}
