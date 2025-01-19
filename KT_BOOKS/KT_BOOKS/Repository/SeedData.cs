using KT_BOOKS.Models;
using KT_BOOKS.Utils;
using Microsoft.EntityFrameworkCore;
using System;

namespace KT_BOOKS.Repository
{
    public class SeedData
    {
        public static void SeedDingData(MyDbContext context)
        {
            // Kiểm tra xem dữ liệu đã tồn tại trong bảng hay không
            context.Database.GetMigrations();
            if (!context.Books.Any() || !context.Authors.Any() || !context.Accounts.Any() || !context.Roles.Any())
            {
                context.Authors.AddRange(
                       new AuthorModel { Name = "Xuân Diệu" },
                       new AuthorModel { Name = "Ngô Tất Tố" },
                       new AuthorModel { Name = "Hồ Xuân Hương" },
                       new AuthorModel { Name = "Nguyễn Du" },
                       new AuthorModel { Name = "Nguyễn Khoa Điềm" }
                );
                context.SaveChanges();

                context.Books.AddRange(
                        new BookModel {  Title = "Tắt đèn", AuthorId = 1, PublishedYear = 2000, Genre = "Truyện Ngắn" },
                        new BookModel { Title = "Hôm Nay Tui Code", AuthorId = 2, PublishedYear = 2010, Genre = "Dân ID" },
                        new BookModel {  Title = "Code là chân ái", AuthorId = 3, PublishedYear = 2020, Genre = "Dân ID" },
						new BookModel { Title = "Hôm nay học gì", AuthorId = 1, PublishedYear = 2000, Genre = "Truyện Ngắn" },
						new BookModel { Title = "Lượm", AuthorId = 2, PublishedYear = 2010, Genre = "Truyện cổ tích" },
						new BookModel { Title = "Thầy bói xem voi", AuthorId = 3, PublishedYear = 2020, Genre = "Truyện cổ tích" },
						new BookModel { Title = "Cây tre trăm đốt", AuthorId = 1, PublishedYear = 2000, Genre = "Truyện cổ tích" },
						new BookModel { Title = "Cậu vàng bán lão hạt mua xe đạp", AuthorId = 2, PublishedYear = 2010, Genre = "Truyện ngắn" },
						new BookModel { Title = "Code không bug không phải tôi", AuthorId = 3, PublishedYear = 2020, Genre = "Dân ID" }
				 );

                context.SaveChanges();

                context.Roles.AddRange(
                       new RoleModel { RoleName = "ADMIN" },
                       new RoleModel { RoleName = "USER" }

                 );
                context.SaveChanges();
                // Tạo dữ liệu cho bảng admin
                context.Accounts.AddRange(

                    new AccountModel { FullName = "ADMIN", UserName = "admin", roleID = 1, Password = PasswordHelper.HashPassword("123")},
                    new AccountModel { FullName = "USER", UserName = "user", roleID = 2, Password = PasswordHelper.HashPassword("123") }

                );
                context.SaveChanges();



            }
        }
    }
}
