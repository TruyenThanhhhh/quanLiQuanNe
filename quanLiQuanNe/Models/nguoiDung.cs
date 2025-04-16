using System.ComponentModel.DataAnnotations;

namespace quanLiQuanNe.Models
{
    public class nguoiDung
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage ="ten dang nhap khong duoc de trong")]
        [StringLength(50,ErrorMessage ="ten dang nhap toi da 50 ki tu")]
        public string userName { get; set; }
        [Required(ErrorMessage = "vui long nhap mat khau")]
        [DataType(DataType.Password)]
        public string passWord { get; set; }
        [Required(ErrorMessage = "vui long nhap ho ten")]
        public string hoTen { get; set; }
        [Required(ErrorMessage = "vui long nhap so dien thoai")]
        [Phone(ErrorMessage ="so dien thoai khong hop le")]
        public string sdt { get; set; }
        public bool isAdmin { get; set; } = false;
      
        public string soDu { get; set; } = "0";
       


    }
}
