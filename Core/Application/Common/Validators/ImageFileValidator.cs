using Microsoft.AspNetCore.Http;

namespace Application.Common.Validators;

public static class ImageFileValidator
{
    private static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
    private static readonly string[] AllowedContentTypes =
    {
        "image/jpeg",
        "image/jpg",
        "image/png",
        "image/gif",
        "image/webp"
    };
    private const long MaxFileSize = 5 * 1024 * 1024; // 5 MB

    public static bool IsValidImage(IFormFile? file)
    {
        if (file == null)
            return false;

        return IsValidSize(file) && IsValidType(file);
    }

    public static bool IsValidSize(IFormFile file)
    {
        return file.Length > 0 && file.Length <= MaxFileSize;
    }

    public static bool IsValidType(IFormFile file)
    {
        if (string.IsNullOrWhiteSpace(file.FileName))
            return false;

        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

        return AllowedExtensions.Contains(extension) &&
               AllowedContentTypes.Contains(file.ContentType.ToLowerInvariant());
    }

    public static string GetErrorMessage()
    {
        return $"La imagen debe ser de tipo {string.Join(", ", AllowedExtensions)} y no superar los {MaxFileSize / 1024 / 1024} MB";
    }

    public static long GetMaxFileSize() => MaxFileSize;

    public static string[] GetAllowedExtensions() => AllowedExtensions;

    public static string[] GetAllowedContentTypes() => AllowedContentTypes;
}
