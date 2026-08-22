using System.Web;

namespace Ecommerce.API.Helpers;

public class GetQrUrl
{
    public static string GetUrl(decimal amount, string transactionId)
    {
        var query = HttpUtility.ParseQueryString(string.Empty);
        query["bank"] = "TPBank";
        query["acc"] = "0000000001";
        query["amount"] = amount.ToString("0");
        query["des"] = transactionId;
        query["template"] = "compact";

        var qrUrl = "https://qr.sepay.vn/img?" + query.ToString();
        
        return qrUrl;
    }
}