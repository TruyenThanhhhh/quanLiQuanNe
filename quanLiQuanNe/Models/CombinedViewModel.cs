using System.Collections.Generic;

namespace quanLiQuanNe.Models
{
    public class CombinedViewModel
    {
        public nguoiDung UserInfo { get; set; }
        public IEnumerable<suDungMay> UsageHistory { get; set; }
    }
}