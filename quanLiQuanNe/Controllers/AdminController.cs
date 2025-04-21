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
        [HttpGet]
        public IActionResult GetUserUpdates()
        {
            var activeUsers = _context.suDungMay
                .Where(s => s.thoiGianKetThuc == null)
                .Select(s => new {
                    SessionId = s.Id,
                    UserId = s.maNguoiDung,
                    MachineId = s.maMay,
                    StartTime = s.thoiGianBatDau,
                    Total = s.tongTien
                })
                .ToList();

            var userIds = activeUsers.Select(u => u.UserId).ToList();
            var users = _context.nguoiDung
                .Where(u => userIds.Contains(u.userName))
                .Select(u => new {
                    UserId = u.userName,
                    Balance = u.soDu
                })
                .ToDictionary(u => u.UserId, u => u.Balance);

            var machineIds = activeUsers.Select(u => int.TryParse(u.MachineId, out int id) ? id : -1)
                .Where(id => id != -1)
                .ToList();

            var machines = _context.mayTinh
                .Where(m => machineIds.Contains(m.id))
                .Select(m => new {
                    Id = m.id,
                    Rate = m.donGia
                })
                .ToDictionary(m => m.Id, m => m.Rate);

            var result = activeUsers.Select(session => {
                int.TryParse(session.MachineId, out int machineId);
                var user = users.GetValueOrDefault(session.UserId, "0");
                var rate = machines.GetValueOrDefault(machineId, "0");

                double remainingTime = 0;
                if (int.TryParse(user, out int balance) &&
                    int.TryParse(rate, out int hourlyRate) &&
                    hourlyRate > 0)
                {
                    remainingTime = (balance / (double)hourlyRate) * 60; // minutes
                }

                return new
                {
                    session.SessionId,
                    session.UserId,
                    session.MachineId,
                    Balance = user,
                    HourlyRate = rate,
                    RemainingTimeInMinutes = remainingTime,
                    session.Total
                };
            }).ToList();

            return Json(result);
        }

        public IActionResult Statistics()
        {
            // Get all active sessions
            var activeSessions = _context.suDungMay
                .Where(s => s.thoiGianKetThuc == null)
                .ToList();

            var userActivities = new List<UserActivityViewModel>();

            foreach (var session in activeSessions)
            {
                // Get user info
                var user = _context.nguoiDung.FirstOrDefault(u => u.userName == session.maNguoiDung);
                if (user == null) continue;

                // Get computer info
                mayTinh computer = null;
                if (int.TryParse(session.maMay, out int mayTinhId))
                {
                    computer = _context.mayTinh.Find(mayTinhId);
                }
                if (computer == null) continue;

                // Calculate remaining time
                double remainingTimeInMinutes = 0;
                if (int.TryParse(user.soDu, out int soDu) &&
                    int.TryParse(computer.donGia, out int donGia) && donGia > 0)
                {
                    remainingTimeInMinutes = (soDu / (double)donGia) * 60; // Convert hours to minutes
                }

                userActivities.Add(new UserActivityViewModel
                {
                    User = user,
                    Session = session,
                    Computer = computer,
                    RemainingTimeInMinutes = remainingTimeInMinutes,
                    IsActive = true
                });
            }

            // Also get recently completed sessions
            var recentCompletedSessions = _context.suDungMay
                .Where(s => s.thoiGianKetThuc != null)
                .OrderByDescending(s => s.thoiGianKetThuc)
                .Take(10)
                .ToList();

            foreach (var session in recentCompletedSessions)
            {
                // Similar logic as above but mark as inactive
                var user = _context.nguoiDung.FirstOrDefault(u => u.userName == session.maNguoiDung);
                if (user == null) continue;

                mayTinh computer = null;
                if (int.TryParse(session.maMay, out int mayTinhId))
                {
                    computer = _context.mayTinh.Find(mayTinhId);
                }
                if (computer == null) continue;

                userActivities.Add(new UserActivityViewModel
                {
                    User = user,
                    Session = session,
                    Computer = computer,
                    RemainingTimeInMinutes = 0,
                    IsActive = false
                });
            }

            var viewModel = new AdminStatisticsViewModel
            {
                ActiveUsers = userActivities
            };

            return View(viewModel);
        }
    }

}
