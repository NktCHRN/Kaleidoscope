using System.Text.Json;

namespace WebApi.IntegrationTests.Serialization;
public static class TestDataSerializer
{
    public static IReadOnlyList<T> Deserialize<T>(string fileName, JsonSerializerOptions options)
    {
        var file = File.ReadAllText(Path.Combine("TestData", "Database", fileName));
        return JsonSerializer.Deserialize<IReadOnlyList<T>>(file, options)!;
    }
}
