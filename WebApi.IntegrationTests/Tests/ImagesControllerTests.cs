using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using WebApi.IntegrationTests.Abstractions;
using WebApi.Models.Responses.Common;
using WebApi.Models.Responses.File;
using Xunit;

namespace WebApi.IntegrationTests.Tests;
public class ImagesControllerTests : ControllerTestsBase
{
    private const string BaseControllerUrl = "api/images/";

    public ImagesControllerTests(CustomWebApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task DownloadImage_ReturnsImage_When_Success()
    {
        var imageName = BlobStorageTestDataHelper.HashToFileName.First().Key;

        var response = await HttpClient.GetAsync($"{BaseControllerUrl}{imageName}");

        response.EnsureSuccessStatusCode();
        response.Content.Should().NotBeNull();
    }

    [Fact]
    public async Task DownloadImage_ReturnsNotFound_When_ImageWasNotFound()
    {
        var imageName = "thisisnotanimage.notanimage";

        var response = await HttpClient.GetAsync($"{BaseControllerUrl}{imageName}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        var content = await response.Content.ReadFromJsonAsync<ErrorResponse>();
        content.Should().NotBeNull();
        content!.ErrorMessage.Should().Be("The file was not found.");
    }

    [Fact]
    public async Task UploadImage_ReturnsOkAndHashedName_When_ImageIsNew()
    {
        var imageName = "iius.drawio.png";
        using var content = new MultipartFormDataContent();
        var fileStream = File.Open(Path.Combine("TestData", "Unseeded", imageName), FileMode.Open);
        content.Add(new StreamContent(fileStream), "file", imageName);

        var requestUri = BaseControllerUrl;
        var request = new HttpRequestMessage(HttpMethod.Post, requestUri) { Content = content };

        var response = await HttpClient.SendAsync(request);

        response.EnsureSuccessStatusCode();
        var responseContent = await response.Content.ReadFromJsonAsync<FileUploadedResponse>();
        responseContent.Should().NotBeNull();
        responseContent!.FileName.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task UploadImage_ReturnsOkAndHashedName_When_ImageAlreadyExists()
    {
        var imageName = BlobStorageTestDataHelper.FileNameToHash.First().Key;
        using var content = new MultipartFormDataContent();
        var fileStream = File.Open(Path.Combine("TestData", "Blob", imageName), FileMode.Open);
        content.Add(new StreamContent(fileStream), "file", imageName);

        var requestUri = BaseControllerUrl;
        var request = new HttpRequestMessage(HttpMethod.Post, requestUri) { Content = content };

        var response = await HttpClient.SendAsync(request);

        response.EnsureSuccessStatusCode();
        var responseContent = await response.Content.ReadFromJsonAsync<FileUploadedResponse>();
        responseContent.Should().NotBeNull();
        responseContent!.FileName.Should().NotBeNullOrEmpty();
    }
}
