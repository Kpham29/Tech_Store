namespace Ecommerce.Web.Models;

public class CartViewModel
{
    public int CartId  { get; set; }
    public int UserId  { get; set; }
    public List<CartItemViewModel> CartItems { get; set; } = new List<CartItemViewModel>();
    public decimal SubTotal => CartItems.Sum(i => i.SubTotal);
    public decimal DeliveryFee  { get; set; }
    public decimal Total => SubTotal + DeliveryFee;
}

public class CartItemViewModel
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