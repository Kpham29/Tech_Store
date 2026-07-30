using Ecommerce.API.Services;
using Ecommerce.BusinessObject;
using Ecommerce.BusinessObject.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly EcommerceDbContext _context;
        private readonly JwtService _jwt;
        
        public AuthController(EcommerceDbContext context, JwtService jwt)
        {
            _context = context;
            _jwt = jwt;
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
                Token = token
            });
        }
    }
}
