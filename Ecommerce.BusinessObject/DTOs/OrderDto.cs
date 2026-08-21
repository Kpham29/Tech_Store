namespace Ecommerce.BusinessObject.DTOs;

public class OrderDto
{
    
}

public class CheckoutRequestDto
{
    public string ShippingAddress  { get; set; }
    public string ShippingPhone  { get; set; }
    public string PaymentMethod  { get; set; }

    public List<CheckoutItem> Items { get; set; } = new List<CheckoutItem>();
}

public class CheckoutItem
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
}

