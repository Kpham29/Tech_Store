using Ecommerce.BusinessObject;
using Ecommerce.BusinessObject.DTOs;
using Ecommerce.BusinessObject.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.API.Controllers
{
    [Route("api/category")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly EcommerceDbContext _context;
        
        public  CategoryController(EcommerceDbContext context)
        {
            _context = context;
        }

        [HttpGet("menu")]
        public async Task<IActionResult> GetMenu()
        {
            var cate = await _context.Categories
                .Include(c => c.CategoryBrands)
                .ThenInclude(b => b.Brand)
                .Select(c => new CategoryMenuDto
                {
                    CategoryId = c.CategoryId,
                    CategoryName = c.CategoryName,
                    Brands = c.CategoryBrands.Select(cb => new BrandMenuDto
                    {
                        BrandId = cb.Brand.BrandId,
                        BrandName = cb.Brand.BrandName
                    }).ToList()
                }).ToListAsync();
            return Ok(cate);
        }
    }
}
