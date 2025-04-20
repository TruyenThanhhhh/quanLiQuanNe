using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using quanLiQuanNe.Models;

namespace quanLiQuanNe.ViewModels
{
    public class SuDungMayIndexViewModel
    {
        public DateTime? TuNgay { get; set; }
        public DateTime? DenNgay { get; set; }
        public string MaMay { get; set; }
        public string MaNguoiDung { get; set; }

        public List<SelectListItem> DanhSachMay { get; set; }
        public List<SelectListItem> DanhSachNguoiDung { get; set; }

        public IEnumerable<suDungMay> DanhSachSuDung { get; set; }
    }
}
