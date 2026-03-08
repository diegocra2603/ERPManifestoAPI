namespace Domain.Contracts.Infrastructure.Services.Files;

public interface IFileService
{
    Task<string> UploadImageAsync(Stream file, string fileName, string? folder = null);

    Task<bool> DeleteImageAsync(string imageUrl);

    Task<string> GetImageUrlAsync(string fileName, int? expireInMinutes = 5);
}