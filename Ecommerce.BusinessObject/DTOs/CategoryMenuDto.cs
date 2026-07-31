namespace Ecommerce.BusinessObject.DTOs;

public class CategoryMenuDto
{
    public int CategoryId { get; set; }
    public string CategoryName { get; set; }
    public List<BrandMenuDto> Brands { get; set; } = new List<BrandMenuDto>();
}

public class BrandMenuDto
{
    public int BrandId { get; set; }
    public string BrandName { get; set; }
}