namespace Ecommerce.API.Helpers;

public class FileValidateHelper
{
    private static readonly byte[] jpgMagic = { 0xFF, 0xD8, 0xFF };
    private static readonly byte[] pngMagic = { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A };

    public static bool IsAvatarValid(IFormFile file)
    {
        var allowedExtension = new[] { ".jpg", ".jpeg", ".png" };
        var extension = Path.GetExtension(file.FileName).ToLower();

        if (!allowedExtension.Contains(extension))
            return false;

        using var stream = file.OpenReadStream();
        var header = new byte[8];
        var bytesRead = stream.Read(header, 0, 8);
        if (bytesRead < 3)
        {
            return false;
        }

        var signature = new Dictionary<string, byte[]>
        {
            [".jpg"] = jpgMagic,
            [".jpeg"] = jpgMagic,
            [".png"] = pngMagic,
        };
        bool check = signature.TryGetValue(extension, out var value) &&
                     header.Take(value.Length).SequenceEqual(value);
        if (!check) return false;
        return true;
    }
}