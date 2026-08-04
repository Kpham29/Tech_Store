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
            user.Email = dto.Email;
            user.Phone = dto.Phone ?? string.Empty;
            user.Address = dto.Address;
            await _context.SaveChangesAsync();
            return Ok(new { message = "Update profile successful" });
        }
    }
}
