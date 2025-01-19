using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KT_BOOKS.Models
{
    public class BookModel
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên sách")]
        [MinLength(5, ErrorMessage = "Tên phải lớn hơn 5 ký tự")]
        public string? Title { get; set; } // tên sách

        [Required(ErrorMessage = "Vui lòng nhập tác giả")]
        public int AuthorId { get; set; } // tác giả

        [ForeignKey("AuthorId")]
        public AuthorModel? AuthorModel { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập năm xuất bản sách")]
        [Range(1800, int.MaxValue, ErrorMessage = "Năm xuất bản phải từ 1800 đến năm hiện tại")]
        public int PublishedYear { get; set; } // năm xuất bản

        [Required(ErrorMessage = "Vui lòng nhập thể loại sách")]
        [MinLength(5, ErrorMessage = "Thể loại phải lớn hơn 5 ký tự")]
        public string? Genre { get; set; } // thể loại

    }
}
