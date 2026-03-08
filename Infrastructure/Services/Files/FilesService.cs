using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Domain.Contracts.Infrastructure.Services.Files;
using Domain.Contracts.Infrastructure.Services.ImageCompressor;
using Microsoft.Extensions.Options;

namespace Services.Files;

public class FilesService : IFileService
{
    private readonly Cloudinary _cloudinary;
    private readonly FilesConfiguration _filesConfiguration;
    private readonly IImageCompressorService _imageCompressor;

    public FilesService(
        IOptions<FilesConfiguration> filesConfiguration,
        IImageCompressorService imageCompressor)
    {
        _filesConfiguration = filesConfiguration.Value;
        _imageCompressor = imageCompressor;

        var account = new Account(
            _filesConfiguration.CloudName,
            _filesConfiguration.ApiKey,
            _filesConfiguration.ApiSecret
        );

        _cloudinary = new Cloudinary(account);
        _cloudinary.Api.Secure = true;
    }

    public async Task<string> UploadImageAsync(Stream file, string fileName, string? folder = null)
    {
        var uploadFolder = folder ?? _filesConfiguration.Folder;

        await using var webpStream = await _imageCompressor.CompressToWebpAsync(file);
        var webpFileName = $"{Path.GetFileNameWithoutExtension(fileName)}.webp";

        var uploadParams = new ImageUploadParams()
        {
            File = new FileDescription(webpFileName, webpStream),
            Folder = uploadFolder,
            PublicId = $"{Path.GetFileNameWithoutExtension(fileName)}_{Guid.NewGuid()}",
            Type = "private",
            Overwrite = false,
            UseFilename = false
        };

        var uploadResult = await _cloudinary.UploadAsync(uploadParams);

        if (uploadResult.Error != null)
        {
            throw new Exception($"Error uploading image: {uploadResult.Error.Message}");
        }

        return uploadResult.PublicId;
    }

    public async Task<bool> DeleteImageAsync(string publicId)
    {
        var deleteParams = new DeletionParams(publicId)
        {
            ResourceType = ResourceType.Image,
            Type = "private"
        };

        var result = await _cloudinary.DestroyAsync(deleteParams);

        return result.Result == "ok";
    }

    public Task<string> GetImageUrlAsync(string publicId, int? expireInMinutes = 5)
    {
        if (string.IsNullOrEmpty(publicId))
        {
            return Task.FromResult(string.Empty);
        }

        try
        {
            // Generar URL privada firmada (como la que funciona de Cloudinary)
            var imageUrl = _cloudinary.Api.UrlImgUp
                .Secure(true)
                .Signed(true)
                .Type("private")
                .BuildUrl(publicId);

            return Task.FromResult(imageUrl);
        }
        catch (Exception ex)
        {
            throw new Exception($"Error generating image URL: {ex.Message}");
        }
    }
}
