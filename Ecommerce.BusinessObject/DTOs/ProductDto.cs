using Ecommerce.BusinessObject.Enums;

namespace Ecommerce.BusinessObject.DTOs;

public class ProductDto
{
    public int ProductId { get; set; }
    public string ProductName { get; set; }
    public decimal Price { get; set; }
    public string Thumbnail { get; set; }
}

public class ProductFilterDto
{
    public string? Keyword  { get; set; }
    public int? CategoryId { get; set; }
    public int? BrandId { get; set; }
    public bool? IsNew { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public string? SortBy { get; set; }
    public ProductStatus? Status { get; set; }

    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}