namespace Ecommerce.Web.Models;

public class CheckoutViewModel
{
    public CustomerProfileViewModel UserProfile { get; set; }
    public List<CartItemViewModel> CartItems { get; set; } = new List<CartItemViewModel>();
    public decimal SubTotal { get; set; }
    public decimal DeliveryFee { get; set; } = 20000;
    public decimal Total => SubTotal + DeliveryFee;
    
    public int? DirectBuyProductId  { get; set; }
}

public class CheckoutSubmitViewModel
{
    public string FullName  { get; set; }
    public string Email { get; set; }
    public string ShippingAddress  { get; set; }
    public string ShippingPhone { get; set; }
    public string PaymentMethod  { get; set; }
    public int? ProductId  { get; set; }
    public int? Quantity   { get; set; }
}