using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using DataAccess.Abstractions;
using DataAccess.Common;
using DataAccess.Options;
using Microsoft.Extensions.Options;

namespace DataAccess.Repositories;
public class BlobRepository : IBlobRepository
{
    private readonly BlobServiceClient _blobServiceClient;
    private readonly BlobStorageOptions _options;

    public BlobRepository(BlobServiceClient blobServiceClient, IOptions<BlobStorageOptions> options)
    {
        _blobServiceClient = blobServiceClient;
        _options = options.Value;
    }

    public async Task<bool> ExistsAsync(string fileName)
    {
        var blobClient = GetBlobClient(fileName);

        return await blobClient.ExistsAsync();
    }

    public async Task<MediaFile?> DownloadPhotoAsync(string fileName)
    {
        var blobClient = GetBlobClient(fileName);

        var fileExists = await blobClient.ExistsAsync();
        if (!fileExists)
        {
            return null;
        }

        var file = await blobClient.DownloadContentAsync();

        return new MediaFile
        {
            ContentType = file.Value.Details.ContentType,
            Name = fileName,
            Data = file.Value.Content
        };
    }

    public async Task UploadPhotoAsync(MediaFile file)
    {
        var blobClient = GetBlobClient(file.Name);

        var fileAlreadyExists = await blobClient.ExistsAsync();
        if (fileAlreadyExists)
        {
            return;
        }

        await blobClient.UploadAsync(file.Data, new BlobUploadOptions
        {
            HttpHeaders = new BlobHttpHeaders
            {
                ContentType = file.ContentType
            }
        });

        return;
    }

    private BlobClient GetBlobClient(string fileName)
    {
        var containerClient =
            _blobServiceClient.GetBlobContainerClient(_options.ContainerName);
        return containerClient.GetBlobClient($"images/{fileName}");
    }
}
