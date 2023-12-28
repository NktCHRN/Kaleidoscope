using Azure.Storage.Blobs;
using WebApi.IntegrationTests.Abstractions;
using WebApi.IntegrationTests.TestDataHelpers;

namespace WebApi.IntegrationTests.Seeders;
public class BlobStorageSeeder : ISeeder
{
    private readonly BlobServiceClient _blobServiceClient;
    private readonly BlobStorageTestDataHelper _testDataHelper;
    private readonly string _blobContainerName;
    private bool _firstSeed = true;

    public BlobStorageSeeder(BlobServiceClient blobServiceClient, BlobStorageTestDataHelper testDataHelper, string containerName)
    {
        _blobServiceClient = blobServiceClient;
        _testDataHelper = testDataHelper;
        _blobContainerName = containerName;
    }

    public async Task SeedAsync()
    {
        var client = _firstSeed 
            ? (await _blobServiceClient.CreateBlobContainerAsync(_blobContainerName)).Value 
            : _blobServiceClient.GetBlobContainerClient(_blobContainerName);

        foreach (var (fileName, hash) in _testDataHelper.FileNameToHash)
        {
            var path = Path.Combine("TestData", "Blob", fileName);
            await client.UploadBlobAsync($"images/{hash}", new BinaryData(File.ReadAllBytes(path)));
        }

        _firstSeed = false;
    }

    public async Task RestoreInitialAsync()
    {
        var client = _blobServiceClient.GetBlobContainerClient(_blobContainerName);
        var blobs = client.GetBlobs();
        foreach (var blob in blobs)
        {
            await client.DeleteBlobAsync(blob.Name);
        }
    }
}
