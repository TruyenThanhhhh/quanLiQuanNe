using Microsoft.AspNetCore.Mvc;
using quanLiQuanNe.Models;
using quanLiQuanNe.Data;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;


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
    }

}
