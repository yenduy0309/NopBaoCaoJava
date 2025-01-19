using KT_BOOKS.Models;
using KT_BOOKS.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KT_BOOKS.Utils;

namespace KT_BOOKS.Services
{
    public class AuthService 
    {
        private readonly MyDbContext _context;

        public AuthService(MyDbContext context)
        {
            _context = context;
        }

        public AccountModel Authenticate(string username, string password)
        { 
             // Mã hóa mật khẩu người dùng nhập vào
            var hashedPassword = PasswordHelper.HashPassword(password);
            // Tìm kiếm người dùng trong cơ sở dữ liệu
            var account = _context.Accounts.FirstOrDefault(a => a.UserName == username && a.Password == hashedPassword);

            if (account == null)
            {
                return null; // Nếu không tìm thấy người dùng
            }

            return account; // Trả về thông tin người dùng
        }
        public RoleModel FindRoleName(int roleID)
        {
            // Tìm kiếm người dùng trong cơ sở dữ liệu
            var role = _context.Roles.Find(Convert.ToInt32(roleID));

            if (role == null)
            {
                return null; // Nếu không tìm thấy người dùng
            }

            return role; // Trả về quyền
        }


    }
}
