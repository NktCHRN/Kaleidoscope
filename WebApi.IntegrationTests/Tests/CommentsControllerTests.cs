using System.Net;
using Microsoft.AspNetCore.WebUtilities;
using System.Net.Http.Json;
using Bogus;
using FluentAssertions;
using WebApi.IntegrationTests.Abstractions;
using WebApi.Models.Common;
using WebApi.Models.Requests.Comment;
using WebApi.Models.Responses.Comment;
using WebApi.Models.Responses.Common;
using Xunit;

namespace WebApi.IntegrationTests.Tests;
public class CommentsControllerTests : ControllerTestsBase
{
    private const string BaseControllerUrl = "api/comments/";

    public CommentsControllerTests(CustomWebApplicationFactory factory) : base(factory)
    {
    }

    private Faker<CreateCommentRequest> _createFaker = new Faker<CreateCommentRequest>()
        .RuleFor(r => r.Text, f => f.Lorem.Sentences(5));

    [Fact]
    public async Task CreateComment_ReturnsCreatedComment_When_Success()
    {
        var request = _createFaker.Generate();
        var postId = Guid.Parse("DCE0B429-6855-401E-F743-08DC05E8BF4A");

        var response = await HttpClient.PostAsJsonAsync($"api/posts/{postId}/comments", request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var content = await response.Content.ReadFromJsonAsync<CommentResponse>();
        content.Should().NotBeNull();
        content!.Text.Should().Be(request.Text);
    }

    private Faker<UpdateCommentRequest> _updateFaker = new Faker<UpdateCommentRequest>()
        .RuleFor(r => r.Text, f => f.Lorem.Sentences(5));

    [Fact]
    public async Task UpdateComment_ReturnsUpdatedComment_When_Success()
    {
        var request = _createFaker.Generate();
        var comment = new Faker().Random.ArrayElement(DatabaseTestDataHelper.Comments.ToArray());
        User.UserId = comment.UserId;

        var response = await HttpClient.PutAsJsonAsync($"{BaseControllerUrl}{comment.Id}", request);

        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadFromJsonAsync<CommentResponse>();
        content.Should().NotBeNull();
        content!.Text.Should().Be(request.Text);
    }

    [Fact]
    public async Task UpdateComment_ReturnsBadRequest_When_UserIsNotAnAuthorOfComment()
    {
        var request = _createFaker.Generate();
        var comment = new Faker().Random.ArrayElement(DatabaseTestDataHelper.Comments.ToArray());
        User.UserId = Guid.NewGuid();

        var response = await HttpClient.PutAsJsonAsync($"{BaseControllerUrl}{comment.Id}", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task DeleteComment_RemovesComment_When_Success()
    {
        var comment = new Faker().Random.ArrayElement(DatabaseTestDataHelper.Comments.ToArray());
        User.UserId = comment.UserId;

        var response = await HttpClient.DeleteAsync($"{BaseControllerUrl}{comment.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task DeleteComment_ReturnsBadRequest_When_UserIsNotAnAuthorOfComment()
    {
        var comment = new Faker().Random.ArrayElement(DatabaseTestDataHelper.Comments.ToArray());
        User.UserId = Guid.NewGuid();

        var response = await HttpClient.DeleteAsync($"{BaseControllerUrl}{comment.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task DeleteComment_ReturnsNotFound_When_CommentIsAlreadyDeleted()
    {
        var commentId = Guid.NewGuid();

        var response = await HttpClient.DeleteAsync($"{BaseControllerUrl}{commentId}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetPagedByPostId_ReturnsCommentsByPostId_When_Success()
    {
        User.IsAuthenticated = false;
        var postId = Guid.Parse("DCE0B429-6855-401E-F743-08DC05E8BF4A");
        var query = new Dictionary<string, string?>()
        {
            {"PerPage","5"},
            {"Page", "1"}
        };

        var response = await HttpClient.GetAsync(QueryHelpers.AddQueryString($"api/posts/{postId}/comments", query));

        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadFromJsonAsync<PagedResponse<CommentResponse, PaginationParametersApiModel>>();
        content.Should().NotBeNull();
        content.Data.Should().NotBeNull();
        content.Data.Should().HaveCountGreaterOrEqualTo(1);
    }
}
