using AutoFixture;
using BusinessLogic.Abstractions;
using BusinessLogic.Dtos;
using BusinessLogic.Exceptions;
using BusinessLogic.Services;
using DataAccess.Abstractions;
using DataAccess.Common;
using FluentAssertions;
using Moq;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using System.Text;
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
        var fileName = _fixture.Create<string>();
        _blobRepository.Setup(b => b.DownloadPhotoAsync(fileName))
            .ReturnsAsync(null as MediaFile);

        var act = () => _imageService.DownloadPhotoAsync(fileName);

        await act.Should().ThrowAsync<EntityNotFoundException>()
            .WithMessage("The file was not found.");
    }

    [Fact]
    public async Task DownloadPhotoAsync_ReturnsMediaFileDto_When_Success()
    {
        var fileName = _fixture.Create<string>();
        var mediaFile = new MediaFile
        {
            ContentType = "image/png",
            Data = new BinaryData(Encoding.UTF8.GetBytes(_fixture.Create<string>())),
            Name = fileName
        };
        _blobRepository.Setup(b => b.DownloadPhotoAsync(fileName))
            .ReturnsAsync(mediaFile);

        var result = await _imageService.DownloadPhotoAsync(fileName);

        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(mediaFile);
    }

    [Fact]
    public async Task UploadPhotoAsync_ThrowsEntityValidationFailedException_When_InvalidImageContentExceptionIsThrownByInfoProvider()
    {
        var mediaFileDto = new MediaFileDto
        {
            ContentType = "image/png",
            Data = new BinaryData(Encoding.UTF8.GetBytes(_fixture.Create<string>())),
            Name = _fixture.Create<string>()
        };
        _imageInfoProvider.Setup(pr => pr.DetectFormatAsync(mediaFileDto.Data))
            .ThrowsAsync(new InvalidImageContentException(string.Empty));

        var act = () => _imageService.UploadPhotoAsync(mediaFileDto);

        await act.Should().ThrowAsync<EntityValidationFailedException>()
            .WithMessage("The file is not an image or this format is not supported.")
            .WithInnerException<EntityValidationFailedException, InvalidImageContentException>();
    }

    [Fact]
    public async Task UploadPhotoAsync_ThrowsEntityValidationFailedException_When_UnknownImageFormatExceptionIsThrownByInfoProvider()
    {
        var mediaFileDto = new MediaFileDto
        {
            ContentType = "image/png",
            Data = new BinaryData(Encoding.UTF8.GetBytes(_fixture.Create<string>())),
            Name = _fixture.Create<string>()
        };
        _imageInfoProvider.Setup(pr => pr.DetectFormatAsync(mediaFileDto.Data))
            .ThrowsAsync(new UnknownImageFormatException(string.Empty));

        var act = () => _imageService.UploadPhotoAsync(mediaFileDto);

        await act.Should().ThrowAsync<EntityValidationFailedException>()
            .WithMessage("The file is not an image or this format is not supported.")
            .WithInnerException<EntityValidationFailedException, UnknownImageFormatException>();
    }

    [Theory]
    [InlineData("image/jpeg", "image/jpeg")]
    [InlineData("application/octet-stream", "image/png")]
    public async Task UploadPhotoAsync_SavesPhoto_When_Success(string initialContentType, string expectedContentType)
    {
        var mediaFileDto = new MediaFileDto
        {
            ContentType = initialContentType,
            Data = new BinaryData(Encoding.UTF8.GetBytes(_fixture.Create<string>())),
            Name = _fixture.Create<string>()
        };
        var expectedName = _fixture.Create<string>();
        _hashedFileNameProvider.Setup(h => h.GenerateName(mediaFileDto.Data, mediaFileDto.Name))
            .Returns(expectedName);
        var formatMock = new Mock<IImageFormat>();
        formatMock.Setup(m => m.DefaultMimeType)
            .Returns("image/png");
        _imageInfoProvider.Setup(pr => pr.DetectFormatAsync(mediaFileDto.Data))
            .ReturnsAsync(formatMock.Object);

        var _ = await _imageService.UploadPhotoAsync(mediaFileDto);

        _blobRepository.Verify(
            b => b.UploadPhotoAsync(
                It.Is<MediaFile>(file => file.Name == expectedName
                    && file.Data == mediaFileDto.Data 
                    && file.ContentType == expectedContentType)), 
            Times.Once);
    }

    [Fact]
    public async Task UploadPhotoAsync_ReturnsFileName_When_Success()
    {
        var mediaFileDto = new MediaFileDto
        {
            ContentType = "image/png",
            Data = new BinaryData(Encoding.UTF8.GetBytes(_fixture.Create<string>())),
            Name = _fixture.Create<string>()
        };
        var expectedName = _fixture.Create<string>();
        _hashedFileNameProvider.Setup(h => h.GenerateName(mediaFileDto.Data, mediaFileDto.Name))
            .Returns(expectedName);
        _imageInfoProvider.Setup(pr => pr.DetectFormatAsync(mediaFileDto.Data))
            .ReturnsAsync(Mock.Of<IImageFormat>());

        var result = await _imageService.UploadPhotoAsync(mediaFileDto);

        result.Should().Be(expectedName);
    }
}
