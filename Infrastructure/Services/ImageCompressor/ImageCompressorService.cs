using Domain.Contracts.Infrastructure.Services.ImageCompressor;
using SkiaSharp;

namespace Services.ImageCompressor;

public class ImageCompressorService : IImageCompressorService
{
    public Task<Stream> CompressToWebpAsync(Stream imageStream, int quality = 80)
    {
        using var original = SKBitmap.Decode(imageStream);

        if (original is null)
            throw new InvalidOperationException("No se pudo decodificar la imagen. El formato no es válido.");

        using var image = SKImage.FromBitmap(original);
        var webpData = image.Encode(SKEncodedImageFormat.Webp, quality);

        var outputStream = new MemoryStream();
        webpData.SaveTo(outputStream);
        outputStream.Position = 0;

        return Task.FromResult<Stream>(outputStream);
    }
}
