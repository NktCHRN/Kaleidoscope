using DataAccess.Common;

namespace DataAccess.Abstractions;
public interface IBlobRepository
{
    Task UploadPhotoAsync(MediaFile file);
    Task<MediaFile?> DownloadPhotoAsync(string fileName);
    Task<bool> ExistsAsync(string fileName);
}
