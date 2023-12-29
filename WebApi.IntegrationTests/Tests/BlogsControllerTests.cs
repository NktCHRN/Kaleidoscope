using System.Net.Http.Json;
using Bogus;
using FluentAssertions;
using WebApi.IntegrationTests.Abstractions;
using WebApi.IntegrationTests.Stubs;
using WebApi.Models.Responses.Blog;
using Xunit;

namespace WebApi.IntegrationTests.Tests;
public class BlogsControllerTests : ControllerTestsBase
{
    private const string BaseControllerUrl = "api/blogs/";

    public BlogsControllerTests(CustomWebApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public void Test1()
    {

    }

    [Fact]
    public void Test2()
    {

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
