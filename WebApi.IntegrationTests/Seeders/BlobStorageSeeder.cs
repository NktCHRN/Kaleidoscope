using Azure.Storage.Blobs;
using Microsoft.Extensions.DependencyInjection;
using WebApi.IntegrationTests.Abstractions;
using WebApi.IntegrationTests.TestDataHelpers;

namespace WebApi.IntegrationTests.Seeders;
public class BlobStorageSeeder : ISeeder
{
    private readonly Func<IServiceScope, BlobServiceClient> _blobServiceClientFactory;
    private readonly BlobStorageTestDataHelper _testDataHelper;
    private readonly string _blobContainerName;
    private bool _firstSeed = true;

    public BlobStorageSeeder(Func<IServiceScope, BlobServiceClient> blobServiceClientFactory, BlobStorageTestDataHelper testDataHelper, string containerName)
    {
        _blobServiceClientFactory = blobServiceClientFactory;
        _testDataHelper = testDataHelper;
        _blobContainerName = containerName;
    }

    public async Task SeedAsync(IServiceScope scope)
    {
        var blobServiceClient = _blobServiceClientFactory(scope);

        var client = _firstSeed 
            ? (await blobServiceClient.CreateBlobContainerAsync(_blobContainerName)).Value 
            : blobServiceClient.GetBlobContainerClient(_blobContainerName);

        foreach (var (fileName, hash) in _testDataHelper.FileNameToHash)
        {
            var path = Path.Combine("TestData", "Blob", fileName);
            await client.UploadBlobAsync($"images/{hash}", new BinaryData(File.ReadAllBytes(path)));
        }

        _firstSeed = false;
    }

    public async Task RestoreInitialAsync(IServiceScope scope)
    {
        var blobServiceClient = _blobServiceClientFactory(scope);
        var client = blobServiceClient.GetBlobContainerClient(_blobContainerName);
        var blobs = client.GetBlobs();
        foreach (var blob in blobs)
        {
            await client.DeleteBlobAsync(blob.Name);
        }

        await SeedAsync(scope);
    }
}
