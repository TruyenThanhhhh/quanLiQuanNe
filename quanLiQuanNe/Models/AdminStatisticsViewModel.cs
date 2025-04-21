namespace quanLiQuanNe.Models
{
    public class AdminStatisticsViewModel
    {
        public List<UserActivityViewModel> ActiveUsers { get; set; }
    }

    public class UserActivityViewModel
    {
        public nguoiDung User { get; set; }
        public suDungMay Session { get; set; }
        public mayTinh Computer { get; set; }
        public double RemainingTimeInMinutes { get; set; }
        public bool IsActive { get; set; }
    }
}
