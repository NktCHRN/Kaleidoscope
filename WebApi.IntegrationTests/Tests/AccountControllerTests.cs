using System.Net;
using System.Net.Http.Json;
using Bogus;
using DataAccess.Entities;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using WebApi.IntegrationTests.Abstractions;
using WebApi.Models.Requests.Account;
using WebApi.Models.Requests.User;
using WebApi.Models.Responses.Account;
using WebApi.Models.Responses.User;
using Xunit;

namespace WebApi.IntegrationTests.Tests;
public class AccountControllerTests : ControllerTestsBase
{
    private const string BaseControllerUrl = "api/account/";

    public AccountControllerTests(CustomWebApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Login_ReturnsTokens_When_Success()
    {
        var request = new LoginAccountRequest
        {
            Email = "nc@gmail.com",
            Password = "String111"
        };

        var response = await HttpClient.PostAsJsonAsync($"{BaseControllerUrl}login", request);

        response.EnsureSuccessStatusCode();
        var responseContent = await response.Content.ReadFromJsonAsync<TokensResponse>();
        responseContent.Should().NotBeNull();
        responseContent!.AccessToken.Should().NotBeNull();
        responseContent.RefreshToken.Should().NotBeNull();
    }

    [Fact]
    public async Task Login_ReturnsUnauthorized_When_Failed()
    {
        var request = new LoginAccountRequest
        {
            Email = "nc@gmail.com",
            Password = new Faker().Internet.Password()
        };

        var response = await HttpClient.PostAsJsonAsync($"{BaseControllerUrl}login", request);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    private readonly Faker<RegisterAccountRequest> _registerAccountRequestFaker = new Faker<RegisterAccountRequest>()
        .RuleFor(r => r.Email, f => f.Person.Email)
        .RuleFor(r => r.Password, f => f.PickRandom("Passw0rd","String123","HelloW0rld"))
        .RuleFor(r => r.Name, f => f.Person.FullName);

    [Fact]
    public async Task Register_ReturnsResponse_When_Success()
    {
        var request = _registerAccountRequestFaker.Generate();

        var response = await HttpClient.PostAsJsonAsync($"{BaseControllerUrl}register", request);

        response.EnsureSuccessStatusCode();
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task RefreshTokens_ReturnsTokensDto_When_Success()
    {
        var refreshToken = DatabaseTestDataHelper.RefreshTokens
            .First(r => r.UserId == Guid.Parse("71A5C868-185A-455C-FAC6-08DC05EB70D4"));
        TimeProvider.ReturnPresetTime(refreshToken.ExpiryTime.AddDays(-1));
        var request = new TokensRequest()
        {
            AccessToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6IjcxYTVjODY4LTE4NWEtNDU1Yy1mYWM2LTA4ZGMwNWViNzBkNCIsImh0dHA6Ly9zY2hlbWFzLnhtbHNvYXAub3JnL3dzLzIwMDUvMDUvaWRlbnRpdHkvY2xhaW1zL25hbWUiOiJuY0BnbWFpbC5jb20iLCJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9lbWFpbGFkZHJlc3MiOiJuY0BnbWFpbC5jb20iLCJodHRwOi8vc2NoZW1hcy5taWNyb3NvZnQuY29tL3dzLzIwMDgvMDYvaWRlbnRpdHkvY2xhaW1zL3JvbGUiOlsiUmVnaXN0ZXJlZFZpZXdlciIsIkF1dGhvciJdLCJuYmYiOjE3MDM4MjkzMzAsImV4cCI6MTcwMzgzMDIzMCwiaXNzIjoiaHR0cHM6Ly9sb2NhbGhvc3Q6NzAwNSIsImF1ZCI6Imh0dHBzOi8vbG9jYWxob3N0OjcwMDUifQ.2IlS9Sv3sUuH4mnuENK_YkKccg3lttOpVKwwZQeNB-M",
            RefreshToken = refreshToken.Token
        };

        var response = await HttpClient.PostAsJsonAsync($"{BaseControllerUrl}tokens/refresh", request);

        response.EnsureSuccessStatusCode();
        var responseContent = await response.Content.ReadFromJsonAsync<TokensResponse>();
        responseContent.Should().NotBeNull();
        responseContent!.AccessToken.Should().NotBeNull();
        responseContent.RefreshToken.Should().NotBeNull();
    }

    [Fact]
    public async Task RevokeToken_RevokesTokenAndReturnsNoContent_When_Success()
    {
        var refreshToken = DatabaseTestDataHelper.RefreshTokens
            .First(r => r.UserId == Guid.Parse("71A5C868-185A-455C-FAC6-08DC05EB70D4"));
        TimeProvider.ReturnPresetTime(refreshToken.ExpiryTime.AddDays(-1));
        var request = new RevokeTokenRequest
        {
            RefreshToken = refreshToken.Token
        };

        var response = await HttpClient.PostAsJsonAsync($"{BaseControllerUrl}tokens/revoke", request);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        var refreshTokenExists = await AccessDatabaseAsync(db => db.Set<RefreshToken>().AnyAsync(r => r.Token == refreshToken.Token));
        refreshTokenExists.Should().BeFalse();
    }

    [Fact]
    public async Task GetDetails_ReturnsUserDetails_When_Success()
    {
        var response = await HttpClient.GetAsync($"{BaseControllerUrl}details");

        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadFromJsonAsync<UserResponse>();
        content.Should().NotBeNull();
    }

    private readonly Faker<UpdateUserRequest> _updateDetailsFaker = new Faker<UpdateUserRequest>()
        .RuleFor(r => r.Name, f => f.Person.FullName)
        .RuleFor(r => r.AvatarLocalFileName, _ => null);

    [Fact]
    public async Task UpdateDetails_ReturnsNewDetails_When_Success()
    {
        var request = _updateDetailsFaker.Generate();

        var response = await HttpClient.PutAsJsonAsync($"{BaseControllerUrl}details", request);

        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadFromJsonAsync<UserResponse>();
        content.Should().NotBeNull();
        content!.Name.Should().Be(request.Name);
        content.Should().BeEquivalentTo(request, opts => opts.ExcludingMissingMembers());
    }
}
