namespace quanLiQuanNe.Models;
using quanLiQuanNe.Models;

public class GamePlayingViewModel
{
    public suDungMay SuDungMay { get; set; }
    public string TenMay { get; set; }
    public string DonGia { get; set; }
    public List<Game> Games { get; set; } // Thêm danh sách game
}