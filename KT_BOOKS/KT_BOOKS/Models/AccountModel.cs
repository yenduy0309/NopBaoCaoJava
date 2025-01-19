using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KT_BOOKS.Models
{
    public class AccountModel
    {
        [Key]
        public int accountID { get; set; }

        [Required(ErrorMessage = "Tên khách bắt buộc")]
        [MinLength(3, ErrorMessage = "Tên khách phải chứa ít nhất 3 ký tự")]
        public string? FullName { get; set; }

        [Required(ErrorMessage = "Tên đăng nhập bắt buộc")]
        [MinLength(3, ErrorMessage = "Tên đăng nhập phải chứa ít nhất 3 ký tự")]
        public string? UserName { get; set; }

        [Required(ErrorMessage = "Mật khẩu bắt buộc")]
        [MinLength(5, ErrorMessage = "Mật khẩu phải chứa ít nhất 5 ký tự")]
        [DataType(DataType.Password)]
        public string? Password { get; set; }

        public int? roleID { get; set; } // Khóa ngoại để liên kết 

        [ForeignKey("roleID")]
        public RoleModel? Role { get; set; }
    }
}
