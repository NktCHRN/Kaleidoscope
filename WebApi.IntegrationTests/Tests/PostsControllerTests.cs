using System.Net;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using System.Text.Json;
using Bogus;
using BusinessLogic.Constants;
using DataAccess.Entities;
using FluentAssertions;
using Microsoft.AspNetCore.WebUtilities;
using WebApi.IntegrationTests.Abstractions;
using WebApi.IntegrationTests.TestDataHelpers;
using WebApi.Models.Common;
using WebApi.Models.Enums;
using WebApi.Models.Requests.Post;
using WebApi.Models.Responses.Comment;
using WebApi.Models.Responses.Common;
using WebApi.Models.Responses.Post;
using Xunit;

namespace WebApi.IntegrationTests.Tests;
public class PostsControllerTests : ControllerTestsBase
{
    private const string BaseControllerUrl = "api/posts/";

    public PostsControllerTests(CustomWebApplicationFactory factory) : base(factory)
    {
    }

    private Faker<CreatePostRequest> _createFaker = new Faker<CreatePostRequest>()
        .RuleFor(r => r.Header, f => f.Lorem.Sentence())
        .RuleFor(r => r.Subheader, f => f.Lorem.Sentence())
        .RuleFor(r => r.PostItems, _ => null!);

    private Faker<ImagePostItemRequest> _imagePostItemFaker = new Faker<ImagePostItemRequest>()
        .RuleFor(r => r.Id, _ => null)
        .RuleFor(r => r.Alt, f => f.Lorem.Sentence())
        .RuleFor(r => r.Description, f => f.Lorem.Sentence())
        .RuleFor(r => r.LocalFileName, _ => null!);

    private Faker<TextPostItemRequest> _textPostItemFaker = new Faker<TextPostItemRequest>()
        .RuleFor(r => r.Id, _ => null)
        .RuleFor(r => r.Text, f => f.Lorem.Text())
        .RuleFor(r => r.TextPostType, f => f.Random.Enum<TextPostType>());

    private readonly JsonSerializerOptions _enumsSerializerOptions = new JsonSerializerOptions
    {
        Converters = { new JsonStringEnumConverter() },
        PropertyNameCaseInsensitive = true
    };

    [Fact]
    public async Task CreatePost_ReturnsCreatedPost_When_Success()
    {
        var request = _createFaker.Generate();
        var image = _imagePostItemFaker.Generate();
        image.LocalFileName = BlobStorageTestDataHelper.HashToFileName.Keys.First();
        var text = _textPostItemFaker.Generate();
        request.PostItems = new List<PostItemRequest>() { image, text };
        var blog = DatabaseTestDataHelper.Blogs.First(b => b.UserId == User.UserId);

        var response = await HttpClient.PostAsJsonAsync($"api/blogs/{blog.Id}/posts", request, _enumsSerializerOptions);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var str = await response.Content.ReadAsStringAsync();
        var content = await response.Content.ReadFromJsonAsync<PostResponse>(_enumsSerializerOptions);
        content.Should().NotBeNull();
        content.PostItems.Should().HaveCount(2);
        content.Should().BeEquivalentTo(
            request, 
            opt => opt.ExcludingMissingMembers()
                .For(r => r.PostItems)
                .Exclude(p => p.Id));
    }

    [Fact]
    public async Task CreatePost_ReturnsForbidden_When_UserHasNotAuthorRole()
    {
        var request = _createFaker.Generate();
        var text = _textPostItemFaker.Generate();
        request.PostItems = new List<PostItemRequest>() { text };
        var blogId = Guid.NewGuid();
        User.Roles.Remove(RolesConstants.Author);

        var response = await HttpClient.PostAsJsonAsync($"api/blogs/{blogId}/posts", request, _enumsSerializerOptions);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    private Faker<UpdatePostRequest> _updateFaker = new Faker<UpdatePostRequest>()
        .RuleFor(r => r.Header, f => f.Lorem.Sentence())
        .RuleFor(r => r.Subheader, f => f.Lorem.Sentence())
        .RuleFor(r => r.PostItems, _ => null!);

    [Fact]
    public async Task UpdatePost_ReturnsCreatedPost_When_Success()
    {
        var post = DatabaseTestDataHelper.Posts.First();
        var blog = DatabaseTestDataHelper.Blogs.First(b => b.Id == post.BlogId);
        var user = DatabaseTestDataHelper.Users.First(u => u.Id == blog.UserId);
        User.UserId = user.Id;
        var request = _updateFaker.Generate();
        var image = _imagePostItemFaker.Generate();
        image.LocalFileName = BlobStorageTestDataHelper.HashToFileName.Keys.First();
        var text = _textPostItemFaker.Generate();
        request.PostItems = new List<PostItemRequest>() { image, text };

        var response = await HttpClient.PutAsJsonAsync($"{BaseControllerUrl}{post.Id}", request, _enumsSerializerOptions);

        response.EnsureSuccessStatusCode();
        var str = await response.Content.ReadAsStringAsync();
        var content = await response.Content.ReadFromJsonAsync<PostResponse>(_enumsSerializerOptions);
        content.Should().NotBeNull();
        content.PostItems.Should().HaveCount(2);
        content.Should().BeEquivalentTo(
            request,
            opt => opt.ExcludingMissingMembers()
                .For(r => r.PostItems)
                .Exclude(p => p.Id));
    }

    [Fact]
    public async Task UpdatePost_ReturnsBadRequest_When_UserIsNotAuthorOfThePost()
    {
        var post = DatabaseTestDataHelper.Posts.First();
        User.UserId = Guid.NewGuid();
        var request = _updateFaker.Generate();
        var text = _textPostItemFaker.Generate();
        request.PostItems = new List<PostItemRequest>() { text };

        var response = await HttpClient.PutAsJsonAsync($"{BaseControllerUrl}{post.Id}", request, _enumsSerializerOptions);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task DeletePost_RemovesPost_When_Success()
    {
        var post = DatabaseTestDataHelper.Posts.First();
        var blog = DatabaseTestDataHelper.Blogs.First(b => b.Id == post.BlogId);
        var user = DatabaseTestDataHelper.Users.First(u => u.Id == blog.UserId);
        User.UserId = user.Id;

        var response = await HttpClient.DeleteAsync($"{BaseControllerUrl}{post.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task DeletePost_ReturnsBadRequest_When_UserIsNotAuthorOfThePost()
    {
        var post = DatabaseTestDataHelper.Posts.First();
        User.UserId = Guid.NewGuid();

        var response = await HttpClient.DeleteAsync($"{BaseControllerUrl}{post.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetPaged_ReturnsPosts_When_Success()
    {
        User.IsAuthenticated = false;
        var expectedPostsCount = 5;
        var query = new Dictionary<string, string?>()
        {
            {"PerPage",expectedPostsCount.ToString()},
            {"Page", "1"}
        };

        var response = await HttpClient.GetAsync(QueryHelpers.AddQueryString(BaseControllerUrl, query));

        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadFromJsonAsync<PagedResponse<PostTitleResponse, PaginationParametersApiModel>>();
        content.Should().NotBeNull();
        content.Data.Should().NotBeNull();
        content.Data.Should().HaveCount(expectedPostsCount);
    }

    [Fact]
    public async Task GetPagedByBlogId_ReturnsPostsByBlogId_When_Success()
    {
        User.IsAuthenticated = false;
        var blogId = Guid.Parse("7E5092CC-E436-4E70-AD02-08DC051FD35E");
        var query = new Dictionary<string, string?>()
        {
            {"PerPage","5"},
            {"Page", "1"}
        };

        var response = await HttpClient.GetAsync(QueryHelpers.AddQueryString($"api/blogs/{blogId}/posts", query));

        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadFromJsonAsync<PagedResponse<PostTitleResponse, PaginationParametersApiModel>>();
        content.Should().NotBeNull();
        content.Data.Should().NotBeNull();
        content.Data.Should().HaveCountGreaterOrEqualTo(1);
    }

    [Fact]
    public async Task GetPostById_ReturnsPost_When_Success()
    {
        User.IsAuthenticated = false;
        var post = new Faker().Random.ArrayElement(DatabaseTestDataHelper.Posts.ToArray());

        var response = await HttpClient.GetAsync($"{BaseControllerUrl}{post.Id}");

        response.EnsureSuccessStatusCode();
        var str = await response.Content.ReadAsStringAsync();
        var content = await response.Content.ReadFromJsonAsync<PostResponse>(_enumsSerializerOptions);
        content.Should().NotBeNull();
        content.Should().BeEquivalentTo(post, opt => opt.ExcludingMissingMembers());
    }
}
