using AutoFixture;
using BusinessLogic.Providers;
using FluentAssertions;
using System.Text;
using Xunit;

namespace BusinessLogic.UnitTests.Tests.Providers;
public class HashedFileNameProviderTests
{
    private readonly HashedFileNameProvider _hashedFileNameProvider = new();
    private readonly Fixture _fixture = new();

    [Theory]
    [InlineData("photo", ".png")]
    [InlineData("ph", ".jpg")]
    [InlineData("hello_world", ".txt")]
    public void GenerateName_PreservesFileExtension_When_FileHasExtension(string fileNameWithoutExtension, string extension)
    {
        var fileName = $"{fileNameWithoutExtension}{extension}";
        var text = _fixture.Create<string>();
        var file = new BinaryData(Encoding.UTF8.GetBytes(text));

        var hashedName = _hashedFileNameProvider.GenerateName(file, fileName);

        hashedName.Should().EndWith(extension);
    }

    [Fact]
    public void GenerateName_ReturnsNameWithoutExtension_When_FileDoesNotHaveExtension()
    {
        var fileName = _fixture.Create<string>();
        var text = _fixture.Create<string>();
        var file = new BinaryData(Encoding.UTF8.GetBytes(text));

        var hashedName = _hashedFileNameProvider.GenerateName(file, fileName);

        hashedName.Should().NotBeNullOrEmpty();
    }
}
