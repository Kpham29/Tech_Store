using Ecommerce.API.Helpers;
using Ecommerce.BusinessObject;
using Ecommerce.BusinessObject.DTOs;
using Ecommerce.BusinessObject.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.API.Controllers
{
    [Route("api/products")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly EcommerceDbContext _context;
        
        public ProductController(EcommerceDbContext context)
        {
            _context = context;
        }

        [HttpGet("filter")]
        public async Task<IActionResult> GetProducts([FromQuery] ProductFilterDto dto)
        {
            var query = _context.Products.Include(p => p.ProductImages).AsQueryable();
            
            if(dto.Status.HasValue)
                query = query.Where(p => p.Status == dto.Status.Value);
            if(!string.IsNullOrWhiteSpace(dto.Keyword))
                query = query.Where(p => p.ProductName.Contains(dto.Keyword));
            if (dto.CategoryId.HasValue)
                query = query.Where(p => p.CategoryId == dto.CategoryId.Value);
            if(dto.BrandId.HasValue)
                query = query.Where(p => p.BrandId == dto.BrandId.Value);
            if (dto.MinPrice.HasValue)
                query = query.Where(p => p.Price >= dto.MinPrice.Value);
            if(dto.MaxPrice.HasValue)
                query = query.Where(p => p.Price <= dto.MaxPrice.Value);
            if (dto.IsNew.HasValue && dto.IsNew.Value)
            {
                var dateThreshold = DateTime.Now.AddDays(-30);
                query = query.Where(p => p.CreatedAt >= dateThreshold);
            }

            query = dto.SortBy switch
            {
                "price_asc" => query.OrderBy(p => p.Price),
                "price_desc" => query.OrderByDescending(p => p.Price),
                "name_asc" => query.OrderBy(p => p.ProductName),
                "name_desc" => query.OrderByDescending(p => p.ProductName),
                "newest" => query.OrderByDescending(p => p.CreatedAt),
                _ => query.OrderByDescending(p => p.CreatedAt)
            };

            var selectQuery = query.Select(p => new ProductDto
            {
                ProductId = p.ProductId,
                ProductName = p.ProductName,
                Price = p.Price,
                Thumbnail = p.ProductImages.FirstOrDefault() != null
                    ? p.ProductImages.FirstOrDefault().Url
                    : "/images/placeholder.png"
            });

            var pagedData = await selectQuery.ToPagedResultAsync(dto.PageIndex, dto.PageSize);
            return Ok(pagedData);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductById(int id)
        {
            var pro = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .Include(p => p.ProductImages)
                .Include(p => p.ProductAttributes)
                .ThenInclude(a => a.Attribute)
                .FirstOrDefaultAsync(p => p.ProductId == id && p.Status == ProductStatus.Active);
            if (pro == null) return NotFound(new { message = "Product does not exist" });
            var dto = new ProductDetailDto
            {
                ProductId = pro.ProductId,
                ProductName = pro.ProductName,
                Price = pro.Price,
                Description = pro.Description ?? string.Empty,
                Quantity = pro.Quantity,
                CategoryName = pro.Category.CategoryName,
                BrandName = pro.Brand.BrandName,
                Thumbnail = pro.ProductImages
                    .Where(i => i.IsThumbnail == true).FirstOrDefault().Url,
                ProductImages = pro.ProductImages
                    .Select(i => i.Url).ToList(),
                Attributes = pro.ProductAttributes
                    .Where(pa => pa.Attribute != null)
                    .ToDictionary(pa => pa.Attribute.AttributeName, pa => pa.AttributeValue)
            };
            return Ok(dto);
        }

        [HttpGet("related-product/{id}")]
        public async Task<IActionResult> GetRelatedProductById(int id)
        {
            var pro = await _context.Products.FirstOrDefaultAsync(p => p.ProductId == id);
            if (pro == null) return NotFound();
            var relatedPros = await _context.Products
                .Include(p => p.ProductImages)
                .Where(p => (p.CategoryId == pro.CategoryId || p.BrandId == pro.BrandId) && p.ProductId != id
                    && p.Status == ProductStatus.Active)
                .Select(p => new ProductDto
                {
                    ProductId = p.ProductId,
                    ProductName = p.ProductName,
                    Price = p.Price,
                    Thumbnail = p.ProductImages.Where(i => i.IsThumbnail == true).FirstOrDefault().Url
                }).ToListAsync();
            return Ok(relatedPros);
        }
    }
}
