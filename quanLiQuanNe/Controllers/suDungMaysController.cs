using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using quanLiQuanNe.Data;
using quanLiQuanNe.Models;
using quanLiQuanNe.ViewModels;

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
        //public async Task<IActionResult> Index()
        //{
        //    return View(await _context.suDungMay.ToListAsync());
        //}

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
        public async Task<IActionResult> ThongKe()
        {
            var danhSach = await _context.suDungMay
                .Where(s => s.thoiGianKetThuc != null && s.tongTien != null)
                .ToListAsync();

            decimal tongThu = danhSach.Sum(s =>
            {
                return decimal.TryParse(s.tongTien, out var tien) ? tien : 0;
            });

            ViewBag.TongThu = tongThu;
            ViewBag.SoLuot = danhSach.Count;
            ViewBag.DoanhThuTheoNgay = danhSach
                .GroupBy(s => s.thoiGianBatDau.Date)
                .Select(g => new
                {
                    Ngay = g.Key,
                    Tong = g.Sum(x => decimal.TryParse(x.tongTien, out var t) ? t : 0)
                })
                .OrderByDescending(x => x.Ngay)
                .ToList();

            return View(danhSach);
        }
        public async Task<IActionResult> Index(DateTime? tuNgay, DateTime? denNgay, string maMay, string maNguoiDung)
        {
            var danhSach = _context.suDungMay.AsQueryable();

            if (tuNgay.HasValue)
                danhSach = danhSach.Where(x => x.thoiGianBatDau.Date >= tuNgay.Value.Date);

            if (denNgay.HasValue)
                danhSach = danhSach.Where(x => x.thoiGianKetThuc.HasValue && x.thoiGianKetThuc.Value.Date <= denNgay.Value.Date);


            if (!string.IsNullOrEmpty(maMay))
                danhSach = danhSach.Where(x => x.maMay == maMay);

            if (!string.IsNullOrEmpty(maNguoiDung))
                danhSach = danhSach.Where(x => x.maNguoiDung == maNguoiDung);

            var model = new SuDungMayIndexViewModel
            {
                TuNgay = tuNgay,
                DenNgay = denNgay,
                MaMay = maMay,
                MaNguoiDung = maNguoiDung,
                DanhSachSuDung = await danhSach.ToListAsync(),

                DanhSachMay = _context.mayTinh
                    .Select(m => new SelectListItem { Value = m.id.ToString(), Text = m.name })
                    .ToList(),

                DanhSachNguoiDung = _context.nguoiDung
                    .Select(nd => new SelectListItem { Value = nd.Id.ToString(), Text = nd.hoTen })
                    .ToList()
            };

            return View(model);
        }


    }
}
