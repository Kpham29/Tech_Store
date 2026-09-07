using System.Security.Claims;
using Ecommerce.API.Helpers;
using Ecommerce.BusinessObject;
using Ecommerce.BusinessObject.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.API.Controllers
{
    [Route("api/customer")]
    [ApiController]
    [Authorize]
    public class CustomerController : ControllerBase
    {
        private readonly EcommerceDbContext _context;
        private readonly IWebHostEnvironment _env;
        
        public CustomerController(EcommerceDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if(string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out int userId))
                return Unauthorized();
            var user = await _context.Users.FindAsync(userId);
            if(user == null) 
                return NotFound("User not found");
            var dto = new CustomerDto
            {
                Username = user.UserName,
                FullName = user.FullName,
                Email = user.Email,
                Phone = user.Phone,
                Address = user.Address,
                Avatar = user.Avatar,
                CreatedAt = user.CreatedAt
            };
            return Ok(dto);
        }

        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile([FromForm] CustomerDto dto, IFormFile? file)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if(string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out int userId))
                return Unauthorized();
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return NotFound();
            if (file != null)
            {
                if(!FileValidateHelper.IsAvatarValid(file))
                    return BadRequest("Invalid file, please upload jpg, png or jpeg file");
                string uploadFolder = Path.Combine(_env.WebRootPath, "avatar");
                if(!Directory.Exists(uploadFolder))
                    Directory.CreateDirectory(uploadFolder);
                string uniqueFileName = Guid.NewGuid().ToString()+ "_" +  file.FileName;
                string filePath = Path.Combine(uploadFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }
                user.Avatar = "/avatar/" + uniqueFileName;
            }
            user.FullName = dto.FullName;
            user.Phone = dto.Phone ?? string.Empty;
            user.Address = dto.Address;
            await _context.SaveChangesAsync();
            return Ok(new { message = "Update profile successful" });
        }

        [HttpPut("change-password")]
        public async Task<IActionResult> ChangePassword([FromForm] ChangePasswordDto dto)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if(string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out int userId))
                return Unauthorized();
            var user = await _context.Users.FindAsync(userId);
            if(user == null) return NotFound();
            if(!BCrypt.Net.BCrypt.Verify(dto.OldPassword, user.Password))
                return BadRequest("Old password is incorrect, please try again");
            if(dto.NewPassword == dto.OldPassword)
                return BadRequest("Old password and new password must be different");
            if(dto.NewPassword != dto.ConfirmNewPassword)
                return BadRequest("New password does not match");
            if (dto.NewPassword.Length < 8 || !dto.NewPassword.Any(char.IsUpper) ||
                !dto.NewPassword.Any(char.IsDigit) ||
                !dto.NewPassword.Any(ch => char.IsLetterOrDigit(ch)))
                return BadRequest(
                    "Your password must be at least 8 characters long, including at least one uppercase letter, one number, and one special character.");
            user.Password = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Password changed successfully" });
        }
    }
}
