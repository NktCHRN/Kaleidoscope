using AutoFixture;
using BusinessLogic.Providers;
using FluentAssertions;
using SixLabors.ImageSharp;
using Xunit;

namespace BusinessLogic.UnitTests.Tests.Providers;
public class ImageInfoProviderTests
{
    private readonly ImageInfoProvider _imageInfoProvider = new();
    private readonly Fixture _fixture = new();

    [Fact]
    public async Task DetectFormatAsync_ThrowsUnknownImageFormatException_When_FileIsNotAnImage()
    {
        var text = _fixture.Create<string>();
        var binaryData = new BinaryData(text);

        var act = () => _imageInfoProvider.DetectFormatAsync(binaryData);

        await act.Should().ThrowAsync<UnknownImageFormatException>();
    }

    [Fact]
    public async Task DetectFormatAsync_ReturnsInfo_When_Success()
    {
        var image = File.ReadAllBytes(Path.Combine("TestData", "usecase.png"));
        var binaryData = new BinaryData(image);

        var result = await _imageInfoProvider.DetectFormatAsync(binaryData);

        result.Should().NotBeNull();
        result.DefaultMimeType.Should().NotBeNullOrEmpty();
    }
}
