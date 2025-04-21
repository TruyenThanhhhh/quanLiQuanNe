// Add this to your existing GamePlayingViewModel class
namespace  quanLiQuanNe.Models;

public class GamePlayingViewModel
{
    public suDungMay SuDungMay { get; set; }
    public string TenMay { get; set; }
    public string DonGia { get; set; }
    public List<Game> Games { get; set; }
    public double RemainingTimeInMinutes { get; set; } // Add this property
}