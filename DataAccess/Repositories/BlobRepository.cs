using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using DataAccess.Abstractions;
using DataAccess.Common;
using DataAccess.Options;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using System.Text;

namespace DataAccess.Repositories;
public class BlobRepository : IBlobRepository
{
    private readonly BlobServiceClient _blobServiceClient;
    private readonly IOptions<BlobStorageOptions> _options;

    public BlobRepository(BlobServiceClient blobServiceClient, IOptions<BlobStorageOptions> options)
    {
        _blobServiceClient = blobServiceClient;
        _options = options;
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

    public async Task<string> UploadPhotoAsync(MediaFile file)
    {
        var fileName = GenerateName(file);

        var blobClient = GetBlobClient(fileName);

        var fileAlreadyExists = await blobClient.ExistsAsync();
        if (fileAlreadyExists)
        {
            return fileName;
        }

        await blobClient.UploadAsync(file.Data, new BlobUploadOptions
        {
            HttpHeaders = new BlobHttpHeaders
            {
                ContentType = file.ContentType
            }
        });

        return fileName;
    }

    private BlobClient GetBlobClient(string fileName)
    {
        var containerClient =
            _blobServiceClient.GetBlobContainerClient(_options.Value.ContainerName);
        return containerClient.GetBlobClient($"images/{fileName}");
    }

    private static string GenerateName(MediaFile file)
    {
        var extension = Path.GetExtension(file.Name);
        using var stream = file.Data.ToStream();
        var hash = SHA256.HashData(stream);
        return $"{Encoding.UTF8.GetString(hash)}{extension}";
    }
}
