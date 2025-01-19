using KT_BOOKS.Models.ViewModels;
using KT_BOOKS.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace KT_BOOKS.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        private readonly AuthService _authService;

        public AuthController(IConfiguration configuration, AuthService authService)
        {
            _configuration = configuration;
            _authService = authService;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginViewModel model)
        {
            // Sử dụng service để xác thực người dùng
            var account = _authService.Authenticate(model.Username, model.Password);
            if (account == null)
            {
                return Unauthorized("Tên đăng nhập hoặc mật khẩu không đúng.");
            }
            var role = _authService.FindRoleName(Convert.ToInt32(account.roleID));
            // Tạo token JWT
            var token = GenerateJwtToken(model.Username, role.RoleName.ToString());
            // Trả về thông tin người dùng cùng với token
            return Ok(new
            {
                FullName = account.FullName,
                roleID = account.roleID,
                role = role,
                Token = token
            });
        }

        private string GenerateJwtToken(string username, string RoleName)
        {
            var claims = new[]
            {
                    new Claim(JwtRegisteredClaimNames.Sub, username),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim("RoleName", RoleName.ToString()) //xác thực quyền truy cập vào endpoint
                 };

            var key = _configuration.GetValue<string>("Jwt:Key");
            if (string.IsNullOrEmpty(key))
            {
                throw new InvalidOperationException("Khóa bảo mật không được cấu hình trong appsettings.json");
            }
            var symmetricKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));

            var creds = new SigningCredentials(symmetricKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(Convert.ToDouble(_configuration["Jwt:ExpireMinutes"])),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
