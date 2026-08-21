namespace Ecommerce.API.Helpers;

public class RandomTransaction
{
    public static string GenerateRandomTransaction(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        Random r = new Random();
        
        return new string(
            Enumerable.Repeat(chars, length)
                .Select(s => s[r.Next(s.Length)])
                .ToArray());
    }
}