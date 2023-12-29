using BusinessLogic.Abstractions;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;

namespace BusinessLogic.Providers;
public class ImageInfoProvider : IImageInfoProvider
{
    public async Task<IImageFormat> DetectFormatAsync(BinaryData imageData)
    {
        return await Image.DetectFormatAsync(imageData.ToStream());
    }
}
