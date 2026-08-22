namespace Ecommerce.BusinessObject.Helpers;

public class SePayResponse
{
    public int status  { get; set; }
    public string error { get; set; }
    public List<SePayTransaction> transactions { get; set; } 
}

public class SePayTransaction
{
    public string id { get; set; }
    public string bank_name { get; set; }
    public string account_number { get; set; }
    public string transaction_date { get; set; }
    public string amount_out { get; set; }
    public string amount_in { get; set; }
    public string transaction_content { get; set; }
    public string reference_number { get; set; }
    public string code  { get; set; }
}