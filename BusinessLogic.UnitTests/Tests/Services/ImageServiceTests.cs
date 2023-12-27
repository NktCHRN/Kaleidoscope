using AutoFixture;
using BusinessLogic.Abstractions;
using BusinessLogic.Services;
using DataAccess.Abstractions;
using Moq;
using Xunit;

namespace BusinessLogic.UnitTests.Tests.Services;
public class ImageServiceTests
{
    private readonly Mock<IBlobRepository> _blobRepository = new();
    private readonly Mock<IImageInfoProvider> _imageInfoProvider = new();
    private readonly Mock<IHashedFileNameProvider> _hashedFileNameProvider = new();

    private readonly ImageService _imageService;

    private readonly Fixture _fixture = new();

    public ImageServiceTests()
    {
        _imageService = new(_blobRepository.Object, _imageInfoProvider.Object, _hashedFileNameProvider.Object);
    }

    [Fact]
    public async Task DownloadPhotoAsync_ThrowsEntityNotFoundException_When_FileWasNotFound()
    {
        Assert.Fail("");
    }
}
