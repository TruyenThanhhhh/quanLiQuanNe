using Microsoft.AspNetCore.Mvc;
using quanLiQuanNe.Models;
using quanLiQuanNe.Data;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;


namespace quanLiQuanNe.Controllers
{
    public class AdminController : Controller
    {
        //public IActionResult Index()
        //{
        //    // Kiểm tra xem đã đăng nhập và có phải admin không
        //    var isAdmin = HttpContext.Session.GetInt32("IsAdmin") == 1;
        //    if (!isAdmin)
        //    {
        //        return RedirectToAction("DangNhap", "Account");
        //    }

        //    ViewBag.UserName = HttpContext.Session.GetString("UserName");
        //    return View();
        //}
        private readonly quanLiQuanNeContext _context;

        public AdminController(quanLiQuanNeContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            try
            {
                var userName = HttpContext.Session.GetString("UserName");
                Console.WriteLine($"Session UserName: {userName}");

                ViewBag.UserName = userName ?? "Unknown";
                return View();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi trong Index: {ex.Message}");
                return RedirectToAction("DangNhap", "Account");
            }
        }
        public IActionResult Index1()
        {
            var users = _context.nguoiDung.ToList();
            return View(users);
        }

        // Hiển thị trang thêm người dùng
        public IActionResult Create()
        {
            return View();
        }

        // Thêm người dùng mới
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("userName,passWord,hoTen,sdt,isAdmin,soDu")] nguoiDung nguoiDung)
        {
            if (ModelState.IsValid)
            {
                _context.Add(nguoiDung);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(nguoiDung);
        }

        // Hiển thị trang sửa thông tin người dùng
        public IActionResult Edit(int id)
        {
            var user = _context.nguoiDung.Find(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        // Cập nhật thông tin người dùng
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, [Bind("Id,userName,passWord,hoTen,sdt,isAdmin,soDu")] nguoiDung nguoiDung)
        {
            if (id != nguoiDung.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(nguoiDung);
                    _context.SaveChanges();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.nguoiDung.Any(e => e.Id == nguoiDung.Id))
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
            return View(nguoiDung);
        }

        // Xóa người dùng
        public IActionResult Delete(int id)
        {
            var user = _context.nguoiDung.Find(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }
        public IActionResult ChucNangUser()
        {
            // Giả sử User.Identity.Name là mã người dùng
            var maNguoiDung = User.Identity.Name;

            // Lấy danh sách lịch sử sử dụng máy tính của người dùng
            var lichSuSuDung = _context.suDungMay
                                       .Where(s => s.maNguoiDung == maNguoiDung)
                                       .ToList();

            return View(lichSuSuDung);
        }

        // Xác nhận xóa người dùng
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var user = _context.nguoiDung.Find(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.nguoiDung.Remove(user);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult NapTien(int id)
        {
            var user = _context.nguoiDung.FirstOrDefault(x => x.Id == id);
            if (user == null) return NotFound();
            return View(user);
        }

        [HttpPost]
        public IActionResult NapTien(int id, int soTien)
        {
            var user = _context.nguoiDung.FirstOrDefault(x => x.Id == id);
            if (user == null) return NotFound();

            // Parse soDu from string to int, default to 0 if parsing fails
            int currentSoDu = 0;
            int.TryParse(user.soDu, out currentSoDu);

            // Add soTien to currentSoDu
            currentSoDu += soTien;

            // Convert back to string and update soDu
            user.soDu = currentSoDu.ToString();

            _context.SaveChanges();

            return RedirectToAction("Index1");
        }

    }

}
