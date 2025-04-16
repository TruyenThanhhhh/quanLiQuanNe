using Microsoft.AspNetCore.Mvc;
using quanLiQuanNe.Models;
using quanLiQuanNe.Data;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System;
using Microsoft.AspNetCore.Mvc.Filters;

namespace quanLiQuanNe.Controllers
{
    public class AdminController : Controller
    {
        private readonly quanLiQuanNeContext _context;

        public AdminController(quanLiQuanNeContext context)
        {
            _context = context;
        }

        [AdminRequired]
        public IActionResult Index()
        {
            try
            {
                var model = new AdminDashboardViewModel
                {
                    NguoiDungs = _context.nguoiDung.ToList(),
                    MayTinhs = _context.mayTinh.ToList()
                };

                return View(model);
            }
            catch (Exception ex)
            {
                // Log error (nên sử dụng ILogger trong thực tế)
                return RedirectToAction("Error", "Home");
            }
        }

        [AdminRequired]
        public IActionResult chucNangAdmin()
        {
            return View();
        }
    }

    // Custom attribute để kiểm tra admin
    public class AdminRequiredAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var isAdmin = context.HttpContext.Session.GetInt32("IsAdmin");
            if (isAdmin != 1)
            {
                context.Result = new RedirectToActionResult("DangNhap", "Account", null);  // Điều hướng về trang đăng nhập nếu không phải admin
            }
        }
    }

    // ViewModel cho trang admin
    public class AdminDashboardViewModel
    {
        public List<nguoiDung> NguoiDungs { get; set; }
        public List<mayTinh> MayTinhs { get; set; }
    }
}
