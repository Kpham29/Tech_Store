using Ecommerce.API.Services;
using Ecommerce.BusinessObject;
using Ecommerce.BusinessObject.DTOs;
using Ecommerce.BusinessObject.Enums;
using Ecommerce.BusinessObject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Ecommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly EcommerceDbContext _context;
        private readonly JwtService _jwt;
        private readonly EmailService _email;
        private readonly IMemoryCache _cache;
        
        public AuthController(EcommerceDbContext context, JwtService jwt, EmailService email, IMemoryCache cache)
        {
            _context = context;
            _jwt = jwt;
            _email = email;
            _cache = cache;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var user = await _context.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.UserName == dto.UserName);
            if (user == null) return Unauthorized("Username is not found");
            if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.Password))
            {
                return Unauthorized("Password is wrong");
            }
            var token = _jwt.GenerateToken(user.UserId.ToString(), user.Email, user.RoleId.ToString());
            return Ok(new
            {
                Username = dto.UserName,
                Role = user.Role.RoleName,
                Avatar = user.Avatar,
                Token = token
            });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            var isPhoneExisted = await _context.Users.AnyAsync(p => p.Phone == dto.Phone);
            var isEmailExisted = await _context.Users.AnyAsync(e => e.Email == dto.Email);
            var isUsernameExisted = await _context.Users.AnyAsync(u => u.UserName == dto.Username);
            if(isEmailExisted)
                return BadRequest("Email is already in use");
            if (dto.Phone.Length >= 12)
                return BadRequest("Invalid phone number format");
            if(isUsernameExisted)
                return BadRequest("Username is already in use");
            if(isPhoneExisted)
                return BadRequest("Phone number is already in use");
            if (dto.Password.Length < 8 || !dto.Password.Any(char.IsUpper) || !dto.Password.Any(char.IsDigit) ||
                !dto.Password.Any(ch => char.IsLetterOrDigit(ch)))
                return BadRequest(
                    "Your password must be at least 8 characters long, including at least one uppercase letter, one number, and one special character.");
            if (dto.Password != dto.ConfirmPassword)
                return BadRequest("Password is not match");
            string hashPassword = BCrypt.Net.BCrypt.HashPassword(dto.Password);
            var newUser = new Users
            {
                UserName = dto.Username,
                Email = dto.Email,
                Password = hashPassword,
                Phone = dto.Phone,
                Address = dto.Address ?? string.Empty,
                RoleId = 3,
                Status = UserStatus.Active,
                CreatedAt = DateTime.Now
            };
            _context.Users.Add(newUser);
            var cart = new Carts
            {
                UserId = newUser.UserId
            };
            _context.Carts.Add(cart);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Register successful" });
        }

        [HttpPost("send-otp")]
        public async Task<IActionResult> SendOtp([FromBody] SendOtpDto dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (user == null)
                return BadRequest("Email is not existed");
            var otp = new Random().Next(100000, 999999).ToString();
            var cacheOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(5));
            _cache.Set(dto.Email, otp, cacheOptions);
            var subject = "Verify your identity";
            var body = $"<p>Hello {user.UserName},</p><p>Your authentication OTP code is: <strong>{otp}</strong></p><p>This OTP code will expire in 5 minutes.</p>";

            try
            {
                _ = Task.Run(() => _email.SendEmailAsync(dto.Email, subject, body));
                return Ok(new {message = "OTP has been sent to your email"});
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [HttpPost("verify-otp")]
        public IActionResult VerifyOtp([FromBody] VerifyOtpDto dto)
        {
            if (_cache.TryGetValue(dto.Email, out string savedOtp))
            {
                if (savedOtp == dto.Otp)
                {
                    _cache.Remove(dto.Email);
                    return Ok(new { message = "OTP verification successful" });
                }

                return BadRequest("Invalid OTP");
            }

            return BadRequest("OTP has expired or email is not found");
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
            if(dto.NewPassword != dto.ConfirmPassword)
                return BadRequest("Password is not match");
            if (dto.NewPassword.Length < 8 || !dto.NewPassword.Any(char.IsUpper) || !dto.NewPassword.Any(char.IsDigit) ||
                !dto.NewPassword.Any(ch => char.IsLetterOrDigit(ch)))
                return BadRequest(
                    "Your password must be at least 8 characters long, including at least one uppercase letter, one number, and one special character.");
            user.Password = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Reset password successful" });
        }
    }
}
