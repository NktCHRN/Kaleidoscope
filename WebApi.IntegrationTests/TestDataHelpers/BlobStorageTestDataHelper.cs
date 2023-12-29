namespace WebApi.IntegrationTests.TestDataHelpers;
public class BlobStorageTestDataHelper
{
    public IReadOnlyDictionary<string, string> HashToFileName => new Dictionary<string, string>
    {
        { "XvhoEkogGuh+9EgFBNDSUkO+1OukEAP9vcaMYhZsE10=.png", "usecase.png" }
    };

    public IReadOnlyDictionary<string, string> FileNameToHash { get; }

    public BlobStorageTestDataHelper()
    {
        FileNameToHash = HashToFileName.ToDictionary(p => p.Value, p => p.Key);
    }
}
