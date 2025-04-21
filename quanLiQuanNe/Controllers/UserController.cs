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
using AspNetCoreGeneratedDocument;
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
        public IActionResult ChucNangUser()
        {
            var userId = HttpContext.Session.GetString("UserName");

            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("DangNhap", "Account");
            }

            var nguoiDung = _context.nguoiDung.FirstOrDefault(u => u.userName == userId);

            if (nguoiDung == null)
            {
                return RedirectToAction("DangNhap", "Account");
            }

            var lichSuSuDung = _context.suDungMay
                .Where(s => s.Id == nguoiDung.Id)
                .ToList();

            var viewModel = new CombinedViewModel
            {
                UserInfo = nguoiDung,
                UsageHistory = lichSuSuDung ?? new List<suDungMay>()
            };

            return View(viewModel);
        }
        [HttpPost]
        public IActionResult DeductBalance()
        {
            var userId = HttpContext.Session.GetString("UserName");
            if (string.IsNullOrEmpty(userId))
            {
                return Json(new { success = false, message = "Người dùng không xác định." });
            }

            // Find current active session
            var suDungMay = _context.suDungMay
                .Where(s => s.maNguoiDung == userId && s.thoiGianKetThuc == null)
                .FirstOrDefault();

            if (suDungMay == null)
            {
                return Json(new { success = false, message = "Không tìm thấy phiên sử dụng." });
            }

            // Get computer rate
            mayTinh mayTinh = null;
            int donGia = 0;
            if (int.TryParse(suDungMay.maMay, out int mayTinhId))
            {
                mayTinh = _context.mayTinh.Find(mayTinhId);
                if (mayTinh != null && int.TryParse(mayTinh.donGia, out int parsedDonGia))
                {
                    donGia = parsedDonGia;
                }
            }

            if (donGia <= 0)
            {
                return Json(new { success = false, message = "Đơn giá không hợp lệ." });
            }

            // Get user and update balance
            var nguoiDung = _context.nguoiDung.FirstOrDefault(u => u.userName == userId);
            if (nguoiDung == null)
            {
                return Json(new { success = false, message = "Không tìm thấy người dùng." });
            }

            if (!int.TryParse(nguoiDung.soDu, out int currentBalance))
            {
                currentBalance = 0;
            }

            // Calculate amount to deduct (1 minute worth of usage)
            double deductionAmount = donGia / 60.0; // Hourly rate divided by 60 minutes

            // Round to the nearest integer
            int amountToDeduct = (int)Math.Ceiling(deductionAmount);

            // Ensure we don't deduct more than available balance
            if (amountToDeduct > currentBalance)
            {
                amountToDeduct = currentBalance;
            }

            // Update balance
            currentBalance -= amountToDeduct;
            nguoiDung.soDu = currentBalance.ToString();

            // Update total spent for this session
            if (!int.TryParse(suDungMay.tongTien, out int currentTotal))
            {
                currentTotal = 0;
            }
            currentTotal += amountToDeduct;
            suDungMay.tongTien = currentTotal.ToString();

            _context.SaveChanges();

            // Check if balance is empty - force logout if it is
            bool shouldLogout = currentBalance <= 0;

            return Json(new
            {
                success = true,
                newBalance = currentBalance,
                shouldLogout = shouldLogout,
                remainingTimeInMinutes = shouldLogout ? 0 : currentBalance / (double)donGia * 60
            });
        }
        public ActionResult PlayingInformation()
        {
            // Nếu có dữ liệu cần truyền sang View thì thêm vào ViewBag hoặc model
            return View();
        }
        public IActionResult GamePlaying()
        {
            var userId = HttpContext.Session.GetString("UserName");
            Console.WriteLine($"GamePlaying - UserId: {userId}");

            if (string.IsNullOrEmpty(userId))
            {
                Console.WriteLine("UserId null, chuyển hướng về DangNhap");
                return RedirectToAction("DangNhap", "Account");
            }

            // Get user information
            var nguoiDung = _context.nguoiDung.FirstOrDefault(u => u.userName == userId);
            if (nguoiDung == null)
            {
                return RedirectToAction("DangNhap", "Account");
            }

            ViewBag.NguoiDung = nguoiDung;

            var suDungMay = _context.suDungMay
                .Where(s => s.maNguoiDung == userId && s.thoiGianKetThuc == null)
                .FirstOrDefault();
            Console.WriteLine($"SuDungMay: {(suDungMay != null ? suDungMay.maMay : "null")}");

            if (suDungMay == null)
            {
                Console.WriteLine("Không tìm thấy phiên sử dụng, chuyển hướng về GameSelection");
                return RedirectToAction("GameSelection", "User");
            }

            // Calculate remaining time
            double remainingTimeInMinutes = 0;
            if (int.TryParse(suDungMay.maMay, out int mayTinhId))
            {
                var mayTinh = _context.mayTinh.Find(mayTinhId);
                Console.WriteLine($"mayTinh: {(mayTinh != null ? mayTinh.name : "null")}");

                if (mayTinh != null)
                {
                    var games = _context.Game.ToList();
                    Console.WriteLine($"Số lượng game: {games.Count}");

                    // Get user balance
                    if (int.TryParse(nguoiDung.soDu, out int soDu) &&
                        int.TryParse(mayTinh.donGia, out int donGia) && donGia > 0)
                    {
                        // Calculate remaining minutes based on current balance
                        remainingTimeInMinutes = (soDu / (double)donGia) * 60; // Convert hours to minutes
                    }

                    var viewModel = new GamePlayingViewModel
                    {
                        SuDungMay = suDungMay,
                        TenMay = mayTinh.name,
                        DonGia = mayTinh.donGia,
                        Games = games,
                        RemainingTimeInMinutes = remainingTimeInMinutes
                    };
                    return View(viewModel);
                }
            }

            Console.WriteLine("Không tìm thấy máy, chuyển hướng về GameSelection");
            return RedirectToAction("GameSelection", "User");
        }

        [HttpPost]
        public IActionResult SelectGameToPlay(int gameId)
        {
            // Lấy userName từ session
            var userId = HttpContext.Session.GetString("UserName");

            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("DangNhap", "Account");
            }

            // Tìm phiên sử dụng máy hiện tại
            var suDungMay = _context.suDungMay
                .Where(s => s.maNguoiDung == userId && s.thoiGianKetThuc == null)
                .FirstOrDefault();

            if (suDungMay == null)
            {
                return RedirectToAction("GameSelection", "User");
            }

            // Tìm game được chọn
            var game = _context.Game.Find(gameId);
            if (game == null)
            {
                return RedirectToAction("GamePlaying", "User");
            }

            // Ở đây bạn có thể lưu thông tin game được chọn vào một bảng khác (nếu cần)
            // Ví dụ: Tạo bảng `GameSession` để lưu thông tin game mà người dùng đang chơi
            // Hoặc chỉ chuyển hướng đến một trang xác nhận

            // Chuyển hướng đến một trang xác nhận hoặc tiếp tục chơi game
            return RedirectToAction("PlayingConfirmation", "User", new { gameName = game.Name });
        }
        public IActionResult PlayingConfirmation(string gameName)
        {
            ViewData["GameName"] = gameName;
            return View();
        }

        public IActionResult GameSelection()
        {
            var userId = HttpContext.Session.GetString("UserName");

            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("DangNhap", "Account");
            }

            var nguoiDung = _context.nguoiDung.FirstOrDefault(u => u.userName == userId);

            if (nguoiDung == null)
            {
                return RedirectToAction("DangNhap", "Account");
            }

            // Lấy danh sách máy tính từ cơ sở dữ liệu
            var danhSachMayTinh = _context.mayTinh.ToList();

            // Truyền danh sách máy tính vào View
            return View(danhSachMayTinh);
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

        [HttpPost]
        public IActionResult SelectGame(int mayTinhId)
        {
            Console.WriteLine($"SelectGame - mayTinhId: {mayTinhId}");

            var mayTinh = _context.mayTinh.Find(mayTinhId);
            Console.WriteLine($"mayTinh: {(mayTinh != null ? mayTinh.name : "null")}, TrangThai: {(mayTinh != null ? mayTinh.trangThai : "null")}");

            if (mayTinh == null || mayTinh.trangThai?.Trim().ToLower() != "trống")
            {
                Console.WriteLine("Điều kiện không thỏa mãn, quay lại GameSelection");
                return RedirectToAction("GameSelection");
            }

            var userId = HttpContext.Session.GetString("UserName");
            Console.WriteLine($"UserId: {userId}");

            if (string.IsNullOrEmpty(userId))
            {
                Console.WriteLine("UserId null, chuyển hướng về DangNhap");
                return RedirectToAction("DangNhap", "Account");
            }

            // Kiểm tra số dư người dùng
            var nguoiDung = _context.nguoiDung.FirstOrDefault(u => u.userName == userId);
            if (nguoiDung == null)
            {
                return RedirectToAction("DangNhap", "Account");
            }

            // Kiểm tra số dư
            if (!int.TryParse(nguoiDung.soDu, out int soDu) || soDu <= 0)
            {
                // Không đủ tiền, hiển thị thông báo
                TempData["ErrorMessage"] = "Số dư tài khoản không đủ. Vui lòng nạp tiền để tiếp tục!";
                return RedirectToAction("GameSelection");
            }

            mayTinh.trangThai = "Hoạt động";

            var suDung = new suDungMay
            {
                maNguoiDung = userId,
                maMay = mayTinhId.ToString(),
                thoiGianBatDau = DateTime.Now,
                thoiGianKetThuc = null,
                tongTien = "0"
            };
            _context.suDungMay.Add(suDung);
            _context.SaveChanges();
            Console.WriteLine("Đã lưu vào suDungMay, chuyển hướng đến GamePlaying");

            return RedirectToAction("GamePlaying", "User");
        }
        [HttpPost]
        public IActionResult Logout()
        {
            try
            {
                // Lấy userName từ session
                var userId = HttpContext.Session.GetString("UserName");

                if (string.IsNullOrEmpty(userId))
                {
                    return RedirectToAction("Login"); // Chuyển hướng nếu không có userId
                }

                // Tìm người dùng
                var nguoiDung = _context.nguoiDung.FirstOrDefault(u => u.userName == userId);
                if (nguoiDung == null)
                {
                    HttpContext.Session.Remove("UserName");
                    return RedirectToAction("Login");
                }

                // Lấy danh sách các máy đang được sử dụng bởi người dùng (chưa có thời gian kết thúc)
                var danhSachSuDungMay = _context.suDungMay
                    .Where(s => s.maNguoiDung == userId && s.thoiGianKetThuc == null)
                    .ToList();

                var thoiGianDangXuat = DateTime.Now;

                foreach (var suDung in danhSachSuDungMay)
                {
                    // Luôn cập nhật thời gian kết thúc khi logout
                    suDung.thoiGianKetThuc = thoiGianDangXuat;

                    // Xử lý máy tính tương ứng
                    if (int.TryParse(suDung.maMay, out int mayTinhId))
                    {
                        var mayTinh = _context.mayTinh.Find(mayTinhId);
                        if (mayTinh != null)
                        {
                            // Cập nhật trạng thái máy thành "TRỐNG"
                            mayTinh.trangThai = "TRỐNG";

                            // Chỉ tính toán tổng tiền nếu chưa có
                            if (string.IsNullOrEmpty(suDung.tongTien))
                            {
                                var thoiGianSuDung = thoiGianDangXuat - suDung.thoiGianBatDau;
                                var soGio = thoiGianSuDung.TotalHours;

                                if (int.TryParse(mayTinh.donGia, out int donGia))
                                {
                                    var tongTien = soGio * donGia;
                                    suDung.tongTien = tongTien.ToString("F0"); // Format không có số thập phân
                                }
                                else
                                {
                                    suDung.tongTien = "0"; // Giá trị mặc định nếu không parse được đơn giá
                                }
                            }
                        }
                    }
                }

                // Lưu tất cả thay đổi vào database
                _context.SaveChanges();

                // Xóa session và chuyển hướng về trang login
                HttpContext.Session.Remove("UserName");
                return RedirectToAction("chucNangUser");
            }
            catch (Exception ex)
            {
                // Ghi log lỗi nếu cần
                // _logger.LogError(ex, "Lỗi khi xử lý đăng xuất");
                return RedirectToAction("Error", "Home");
            }
        }
    }
}
