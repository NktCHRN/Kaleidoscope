using BusinessLogic.Abstractions;
using BusinessLogic.Dtos;
using BusinessLogic.Exceptions;
using DataAccess.Abstractions;
using DataAccess.Common;
using SixLabors.ImageSharp;

namespace BusinessLogic.Services;
public class ImageService : IImageService
{
    private readonly IBlobRepository _blobRepository;
    private readonly IImageInfoProvider _imageInfoProvider;
    private readonly IHashedFileNameProvider _hashedFileNameProvider;

    public ImageService(IBlobRepository blobRepository, IImageInfoProvider imageInfoProvider, IHashedFileNameProvider hashedFileNameProvider)
    {
        _blobRepository = blobRepository;
        _imageInfoProvider = imageInfoProvider;
        _hashedFileNameProvider = hashedFileNameProvider;
    }

    public async Task<MediaFileDto> DownloadPhotoAsync(string fileName)
    {
        var file = await _blobRepository.DownloadPhotoAsync(fileName) 
            ?? throw new EntityNotFoundException("The file was not found.");

        return new MediaFileDto
        {
            Data = file.Data,
            ContentType = file.ContentType,
            Name = file.Name,
        };
    }

    public async Task<string> UploadPhotoAsync(MediaFileDto fileDto)
    {
        try
        {
            var imageInfo = await _imageInfoProvider.DetectFormatAsync(fileDto.Data);
            var fileName = _hashedFileNameProvider.GenerateName(fileDto.Data, fileDto.Name);
            var file = new MediaFile
            {
                ContentType = fileDto.ContentType.StartsWith("image/") ? fileDto.ContentType : imageInfo.DefaultMimeType,
                Data = fileDto.Data,
                Name = fileName,
            };
            await _blobRepository.UploadPhotoAsync(file);
            return fileName;
        }
        catch (InvalidImageContentException ex)
        {
            throw new EntityValidationFailedException("The file is not an image or this format is not supported.", ex);
        }
        catch (UnknownImageFormatException ex)
        {
            throw new EntityValidationFailedException("The file is not an image or this format is not supported.", ex);
        }
    }
}
