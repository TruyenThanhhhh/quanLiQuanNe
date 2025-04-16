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
    public class suDungMaysController : Controller
    {
        private readonly quanLiQuanNeContext _context;

        public suDungMaysController(quanLiQuanNeContext context)
        {
            _context = context;
        }

        // GET: suDungMays
        public async Task<IActionResult> Index()
        {
            return View(await _context.suDungMay.ToListAsync());
        }

        // GET: suDungMays/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var suDungMay = await _context.suDungMay
                .FirstOrDefaultAsync(m => m.Id == id);
            if (suDungMay == null)
            {
                return NotFound();
            }

            return View(suDungMay);
        }

        // GET: suDungMays/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: suDungMays/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,maNguoiDung,maMay,thoiGianBatDau,thoiGianKetThuc,tongTien")] suDungMay suDungMay)
        {
            if (ModelState.IsValid)
            {
                _context.Add(suDungMay);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(suDungMay);
        }

        // GET: suDungMays/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var suDungMay = await _context.suDungMay.FindAsync(id);
            if (suDungMay == null)
            {
                return NotFound();
            }
            return View(suDungMay);
        }

        // POST: suDungMays/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,maNguoiDung,maMay,thoiGianBatDau,thoiGianKetThuc,tongTien")] suDungMay suDungMay)
        {
            if (id != suDungMay.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(suDungMay);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!suDungMayExists(suDungMay.Id))
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
            return View(suDungMay);
        }

        // GET: suDungMays/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var suDungMay = await _context.suDungMay
                .FirstOrDefaultAsync(m => m.Id == id);
            if (suDungMay == null)
            {
                return NotFound();
            }

            return View(suDungMay);
        }

        // POST: suDungMays/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var suDungMay = await _context.suDungMay.FindAsync(id);
            if (suDungMay != null)
            {
                _context.suDungMay.Remove(suDungMay);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool suDungMayExists(int id)
        {
            return _context.suDungMay.Any(e => e.Id == id);
        }
    }
}
