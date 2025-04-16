using Microsoft.AspNetCore.Mvc;
using quanLiQuanNe.Models;
using quanLiQuanNe.Data;
using Microsoft.AspNetCore.Http;
using System.Linq;

namespace quanLiQuanNe.Controllers
{
    public class AccountController : Controller
    {
        private readonly quanLiQuanNeContext _context;

        public AccountController(quanLiQuanNeContext context)
        {
            _context = context;
        }

        // GET: Display Login Page
        public IActionResult DangNhap()
        {
            return View();
        }

        // POST: Handle Login
        [HttpPost]
        [HttpPost]
        [HttpPost]
        public IActionResult DangNhap(nguoiDung model)
        {
            if (ModelState.IsValid)
            {
                var user = _context.nguoiDung
                    .FirstOrDefault(u => u.userName == model.userName && u.passWord == model.passWord);

                if (user != null)
                {
                    // Lưu session
                    HttpContext.Session.SetString("UserName", user.userName);
                    HttpContext.Session.SetInt32("IsAdmin", user.isAdmin ? 1 : 0);

                    // Kiểm tra role và chuyển hướng
                    if (user.isAdmin)
                    {
                        Console.WriteLine($"Admin session: {HttpContext.Session.GetInt32("IsAdmin")}");
                        return RedirectToAction("Index", "Admin"); // Đảm bảo tên controller đúng
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }

                ModelState.AddModelError("", "Tên đăng nhập hoặc mật khẩu không đúng.");
            }

            return View(model);
        }
        public IActionResult DangXuat()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("DangNhap", "Account");
        }
        // GET: Display Register Page
        [HttpGet]
        public IActionResult DangKy()
        {
            return View();
        }

        // POST: Handle Registration
        [HttpPost]
        public IActionResult DangKy(nguoiDung model)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra xem username đã tồn tại chưa
                var existingUser = _context.nguoiDung.FirstOrDefault(u => u.userName == model.userName);
                if (existingUser != null)
                {
                    ModelState.AddModelError("userName", "Tên đăng nhập đã tồn tại.");
                    return View(model);
                }

                // Mặc định không phải admin khi đăng ký
                model.isAdmin = false;

                _context.nguoiDung.Add(model);
                _context.SaveChanges();

                // Sau khi đăng ký thành công, chuyển sang trang đăng nhập
                return RedirectToAction("DangNhap", "Account");
            }

            return View(model);
        }
    }
}
