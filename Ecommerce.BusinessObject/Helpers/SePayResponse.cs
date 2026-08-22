namespace Ecommerce.BusinessObject.Helpers;

public class SePayResponse
{
    public List<SePayTransaction> transactions { get; set; } 
}

public class SePayTransaction
{
    public decimal amount_in { get; set; }
    public string transaction_content { get; set; }
}