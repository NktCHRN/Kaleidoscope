using BusinessLogic.Dtos;

namespace BusinessLogic.Abstractions;
public interface IImageService
{
    Task<string> UploadPhotoAsync(MediaFileDto file);
    Task<MediaFileDto> DownloadPhotoAsync(string fileName);
}
