//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using quanLiQuanNe.Data;
//using quanLiQuanNe.Models;

//public class UserController : Controller
//{
//    private readonly quanLiQuanNeContext _context;

//    public UserController(quanLiQuanNeContext context)
//    {
//        _context = context;
//    }

//    public IActionResult ChucNangUser()
//    {
//        // Giả sử User.Identity.Name là mã người dùng
//        var maNguoiDung = User.Identity.Name;

//        // Lấy danh sách lịch sử sử dụng máy tính của người dùng
//        var lichSuSuDung = _context.suDungMay
//                                   .Where(s => s.maNguoiDung == maNguoiDung)
//                                   .ToList();

//        return View(lichSuSuDung);
//    }
//}
using Microsoft.AspNetCore.Mvc;
using quanLiQuanNe.Models;
using quanLiQuanNe.Data;
using Microsoft.EntityFrameworkCore;
namespace quanLiQuanNe.Controllers
{
    public class UserController : Controller
    {
        private readonly quanLiQuanNeContext _context;

        public UserController(quanLiQuanNeContext context)
        {
            _context = context;
        }

        // Hiển thị danh sách người dùng
        public IActionResult Index()
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

    }
}
