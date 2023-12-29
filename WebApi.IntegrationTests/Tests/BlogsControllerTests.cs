using System.Net;
using System.Net.Http.Json;
using Bogus;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using WebApi.IntegrationTests.Abstractions;
using WebApi.Models.Requests.Blog;
using WebApi.Models.Responses.Blog;
using Xunit;

namespace WebApi.IntegrationTests.Tests;
public class BlogsControllerTests : ControllerTestsBase
{
    private const string BaseControllerUrl = "api/blogs/";

    public BlogsControllerTests(CustomWebApplicationFactory factory) : base(factory)
    {
    }

    private readonly Faker<CreateBlogRequest> _createFaker = new Faker<CreateBlogRequest>()
        .RuleFor(r => r.Tag, f => f.PickRandom("sblog3213", "nrew", "fdd2323"))
        .RuleFor(r => r.Description, f => f.Random.String2(100, 1000));

    [Fact]
    public async Task Create_ReturnsCreatedBlog_When_Success()
    {
        User.UserId = Guid.Parse("84B50EE6-DCAB-469D-FA09-08DC04F40D0B");
        var request = _createFaker.Generate();

        var response = await HttpClient.PostAsJsonAsync(BaseControllerUrl, request);

        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadFromJsonAsync<BlogResponse>();
        content.Should().NotBeNull();
        content.Should().BeEquivalentTo(request, opt => opt.ExcludingMissingMembers());
    }

    [Fact]
    public async Task Create_ReturnsConflict_When_BlogWithTagAlreadyExists()
    {
        User.UserId = Guid.Parse("84B50EE6-DCAB-469D-FA09-08DC04F40D0B");
        var request = _createFaker.Generate();
        request.Tag = DatabaseTestDataHelper.Blogs.First().Tag;

        var response = await HttpClient.PostAsJsonAsync(BaseControllerUrl, request);

        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task Create_ReturnsConflict_When_UserAlreadyHasABlog()
    {
        var request = _createFaker.Generate();
        request.Tag = DatabaseTestDataHelper.Blogs.First().Tag;

        var response = await HttpClient.PostAsJsonAsync(BaseControllerUrl, request);

        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    private readonly Faker<UpdateBlogRequest> _updateFaker = new Faker<UpdateBlogRequest>()
        .RuleFor(r => r.Tag, f => f.PickRandom("sblog3213", "nrew", "fdd2323"))
        .RuleFor(r => r.Description, f => f.Random.String2(100, 1000))
        .RuleFor(r => r.Name, f => f.Internet.UserName())
        .RuleFor(r => r.AvatarLocalFileName, _ => null);

    [Fact]
    public async Task Update_UpdatesAndReturnsBlog_When_Success()
    {
        var request = _updateFaker.Generate();
        var blogId = DatabaseTestDataHelper.Blogs.First(b => b.UserId == User.UserId).Id;

        var response = await HttpClient.PutAsJsonAsync($"{BaseControllerUrl}{blogId}", request);

        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadFromJsonAsync<BlogResponse>();
        content.Should().NotBeNull();
        content.Should().BeEquivalentTo(request, opt => opt.ExcludingMissingMembers());
    }

    [Fact]
    public async Task Update_ReturnsConflict_When_BlogWithTagAlreadyExists()
    {
        var request = _updateFaker.Generate();
        var blogId = DatabaseTestDataHelper.Blogs.First(b => b.UserId == User.UserId).Id;
        request.Tag = DatabaseTestDataHelper.Blogs.First(b => b.UserId != User.UserId).Tag;

        var response = await HttpClient.PostAsJsonAsync(BaseControllerUrl, request);

        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task GetByTag_ReturnsBlog_When_Success()
    {
        User.IsAuthenticated = false;
        var blog = new Faker().Random.ArrayElement(DatabaseTestDataHelper.Blogs.ToArray());

        var response = await HttpClient.GetAsync($"{BaseControllerUrl}{blog.Tag}");

        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadFromJsonAsync<BlogResponse>();
        content.Should().BeEquivalentTo(blog, options => options.ExcludingMissingMembers());
    }
}
