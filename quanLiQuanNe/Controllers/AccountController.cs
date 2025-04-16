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

        // GET: Hiển thị trang đăng nhập
        public IActionResult DangNhap()
        {
            if (TempData["ErrorMessage"] != null)
            {
                ViewBag.ErrorMessage = TempData["ErrorMessage"].ToString();
            }

            return View();
        }

        // POST: Xử lý đăng nhập
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DangNhap(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = _context.nguoiDung
                    .FirstOrDefault(u => u.userName == model.userName && u.passWord == model.passWord);

                if (user != null)
                {
                    HttpContext.Session.SetString("UserName", user.userName);
                    HttpContext.Session.SetInt32("IsAdmin", user.isAdmin ? 1 : 0);

                    if (user.isAdmin)
                        return RedirectToAction("Index", "Admin");
                    else
                        return RedirectToAction("chucNangUser", "User");
                }
                TempData["ErrorMessage"] = "Tên đăng nhập hoặc mật khẩu không đúng.";
            }
            return View(model);
        }

        // Phương thức để kiểm tra mật khẩu
        private bool VerifyPassword(string inputPassword, string storedHashedPassword)
        {
            // Implement password verification using hashing (e.g., BCrypt, PBKDF2)
            // Ví dụ sử dụng BCrypt:
            // return BCrypt.Verify(inputPassword, storedHashedPassword);

            // Hoặc tạm thời cho test:
            return inputPassword == storedHashedPassword; // Không dùng cách này trong production!
        }

        // GET: Đăng xuất
        public IActionResult DangXuat()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("DangNhap", "Account");
        }

        // GET: Hiển thị trang đăng ký
        [HttpGet]
        public IActionResult DangKy()
        {
            return View();
        }

        // POST: Xử lý đăng ký
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DangKy(nguoiDung model)
        {
            if (ModelState.IsValid)
            {
                var existingUser = _context.nguoiDung.FirstOrDefault(u => u.userName == model.userName);
                if (existingUser != null)
                {
                    ModelState.AddModelError("userName", "Tên đăng nhập đã tồn tại.");
                    return View(model);
                }

                // Không mã hóa mật khẩu
                model.isAdmin = false; // Mặc định không phải admin

                _context.nguoiDung.Add(model);
                _context.SaveChanges();

                TempData["SuccessMessage"] = "Đăng ký thành công! Vui lòng đăng nhập.";
                return RedirectToAction("DangNhap", "Account");
            }

            return View(model);
        }
    }
}
