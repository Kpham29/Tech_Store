namespace Ecommerce.BusinessObject.DTOs;

public class CartDto
{
    public int CartId  { get; set; }
    public int UserId  { get; set; }
    public List<CartItemDto> CartItems { get; set; } = new List<CartItemDto>();
    public decimal SubTotal => CartItems.Sum(i => i.SubTotal);
    public decimal DeliveryFee  { get; set; }
    public decimal Total => SubTotal + DeliveryFee;
}

public class CartItemDto
{
    public int CartItemId  { get; set; }
    public int CartId   { get; set; }
    public int ProductId { get; set; }
    public string ProductName  { get; set; }
    public string Thumbnail { get; set; }
    public int Quantity  { get; set; }
    public decimal Price  { get; set; }
    public decimal SubTotal => Price * Quantity;
}

public class AddCartItemDto
{
    public int ProductId  { get; set; }
    public int Quantity { get; set; } = 1;
}

public class UpdateCartDto
{
    public List<UpdateCartItemDto> Items { get; set; } = new List<UpdateCartItemDto>();
}
public class UpdateCartItemDto
{
    public int CartItemId { get; set; }
    public int Quantity { get; set; }
}