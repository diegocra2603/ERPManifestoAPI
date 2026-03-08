namespace Domain.Contracts.Infrastructure.Services.ImageCompressor;

public interface IImageCompressorService
{
    Task<Stream> CompressToWebpAsync(Stream imageStream, int quality = 80);
}
