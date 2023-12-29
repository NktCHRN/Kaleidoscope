using SixLabors.ImageSharp.Formats;

namespace BusinessLogic.Abstractions;
public interface IImageInfoProvider
{
    Task<IImageFormat> DetectFormatAsync(BinaryData imageData);
}
