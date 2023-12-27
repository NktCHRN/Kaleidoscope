using AutoFixture;
using BusinessLogic.Abstractions;
using BusinessLogic.Dtos;
using BusinessLogic.Exceptions;
using BusinessLogic.Services;
using BusinessLogic.UnitTests.Customizations;
using DataAccess.Abstractions;
using DataAccess.Entities;
using DataAccess.Specifications;
using FluentAssertions;
using Moq;
using System.Security;
using System.Security.Claims;
using Xunit;

namespace BusinessLogic.UnitTests.Tests.Services;
public class RefreshTokenServiceTests
{
    private readonly Mock<IJwtTokenProvider> _jwtTokenProvider = new();
    private readonly Mock<IRepository<RefreshToken>> _refreshTokenRepository = new();
    private readonly Mock<TimeProvider> _timeProvider = new();

    private readonly RefreshTokenService _refreshTokenService;

    private readonly Fixture _fixture = new();

    public RefreshTokenServiceTests()
    {
        _fixture.Customize(new DomainCustomization());

        _refreshTokenService = new(_jwtTokenProvider.Object, _refreshTokenRepository.Object, _timeProvider.Object);
    }

    [Fact]
    public async Task Refresh_ThrowsEntityValidationException_When_SecurityExceptionIsThrowsByTokenProvider()
    {
        var dto = _fixture.Create<TokensDto>();
        _jwtTokenProvider.Setup(p => p.GetPrincipalFromExpiredToken(dto.AccessToken))
            .Throws(new SecurityException());

        var act = () => _refreshTokenService.Refresh(dto);

        await act.Should()
            .ThrowAsync<EntityValidationFailedException>()
            .WithInnerException<EntityValidationFailedException, SecurityException>();
    }

    [Fact]
    public async Task Refresh_ThrowsEntityValidationException_When_RefreshTokenIsNotInRepository()
    {
        var dto = _fixture.Create<TokensDto>();
        var userId = _fixture.Create<Guid>();
        var principalMock = new Mock<ClaimsPrincipal>();
        principalMock.Setup(p => p.FindFirst(ClaimTypes.NameIdentifier))
            .Returns(new Claim(ClaimTypes.NameIdentifier, userId.ToString()));
        _jwtTokenProvider.Setup(p => p.GetPrincipalFromExpiredToken(dto.AccessToken))
            .Returns(principalMock.Object);
        _refreshTokenRepository.Setup(r => r.FirstOrDefaultAsync(It.IsAny<RefreshTokenByUserIdAndTokenSpec>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(null as RefreshToken);

        var act = () => _refreshTokenService.Refresh(dto);

        await act.Should()
            .ThrowAsync<EntityValidationFailedException>()
            .WithMessage("Invalid client request");
    }

    [Fact]
    public async Task Refresh_ThrowsEntityValidationException_When_RefreshTokenIsExpired()
    {
        var dto = _fixture.Create<TokensDto>();
        var userId = _fixture.Create<Guid>();
        var refreshToken = _fixture.Create<RefreshToken>();
        var principalMock = new Mock<ClaimsPrincipal>();
        principalMock.Setup(p => p.FindFirst(ClaimTypes.NameIdentifier))
            .Returns(new Claim(ClaimTypes.NameIdentifier, userId.ToString()));
        _jwtTokenProvider.Setup(p => p.GetPrincipalFromExpiredToken(dto.AccessToken))
            .Returns(principalMock.Object);
        _refreshTokenRepository.Setup(r => r.FirstOrDefaultAsync(It.IsAny<RefreshTokenByUserIdAndTokenSpec>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(refreshToken);
        _timeProvider.Setup(t => t.GetUtcNow())
            .Returns(refreshToken.ExpiryTime.AddDays(1));

        var act = () => _refreshTokenService.Refresh(dto);

        await act.Should()
            .ThrowAsync<EntityValidationFailedException>()
            .WithMessage("Invalid client request");
    }

    [Fact]
    public async Task Refresh_UpdatesRefreshToken_When_Success()
    {
        var dto = _fixture.Create<TokensDto>();
        var userId = _fixture.Create<Guid>();
        var refreshToken = _fixture.Create<RefreshToken>();
        var newRefreshTokenString = _fixture.Create<string>();
        var principalMock = new Mock<ClaimsPrincipal>();
        principalMock.Setup(p => p.FindFirst(ClaimTypes.NameIdentifier))
            .Returns(new Claim(ClaimTypes.NameIdentifier, userId.ToString()));
        _jwtTokenProvider.Setup(p => p.GetPrincipalFromExpiredToken(dto.AccessToken))
            .Returns(principalMock.Object);
        _refreshTokenRepository.Setup(r => r.FirstOrDefaultAsync(It.IsAny<RefreshTokenByUserIdAndTokenSpec>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(refreshToken);
        _timeProvider.Setup(t => t.GetUtcNow())
            .Returns(refreshToken.ExpiryTime.AddDays(-1));
        _jwtTokenProvider.Setup(p => p.GenerateRefreshToken())
            .Returns(newRefreshTokenString);

        _ = await _refreshTokenService.Refresh(dto);

        _refreshTokenRepository.Verify(
            r => r.UpdateAsync(
                It.Is<RefreshToken>(r => r.ExpiryTime == refreshToken.ExpiryTime && r.Token == newRefreshTokenString),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Refresh_ReturnsNewTokens_When_Success()
    {
        var dto = _fixture.Create<TokensDto>();
        var userId = _fixture.Create<Guid>();
        var refreshToken = _fixture.Create<RefreshToken>();
        var expectedTokens = _fixture.Create<TokensDto>();
        var principalMock = new Mock<ClaimsPrincipal>();
        principalMock.Setup(p => p.FindFirst(ClaimTypes.NameIdentifier))
            .Returns(new Claim(ClaimTypes.NameIdentifier, userId.ToString()));
        _jwtTokenProvider.Setup(p => p.GetPrincipalFromExpiredToken(dto.AccessToken))
            .Returns(principalMock.Object);
        _refreshTokenRepository.Setup(r => r.FirstOrDefaultAsync(It.IsAny<RefreshTokenByUserIdAndTokenSpec>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(refreshToken);
        _timeProvider.Setup(t => t.GetUtcNow())
            .Returns(refreshToken.ExpiryTime.AddDays(-1));
        _jwtTokenProvider.Setup(p => p.GenerateRefreshToken())
            .Returns(expectedTokens.RefreshToken);
        _jwtTokenProvider.Setup(p => p.GenerateAccessToken(It.IsAny<IEnumerable<Claim>>()))
            .Returns(expectedTokens.AccessToken);

        var result = await _refreshTokenService.Refresh(dto);

        result.Should().Be(expectedTokens);
    }

    [Fact]
    public async Task Revoke_ThrowsEntityNotFoundException_When_UserOrTokenWasNotFound()
    {
        var userId = _fixture.Create<Guid>();
        var refreshToken = _fixture.Create<string>();
        _refreshTokenRepository.Setup(r => r.FirstOrDefaultAsync(It.IsAny<RefreshTokenByUserIdAndTokenSpec>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(null as RefreshToken);

        var act = () => _refreshTokenService.Revoke(userId, refreshToken);

        await act.Should()
            .ThrowAsync<EntityNotFoundException>()
            .WithMessage("User or refresh token was not found");
    }

    [Fact]
    public async Task Revoke_DeletesRefreshToken_When_Success()
    {
        var userId = _fixture.Create<Guid>();
        var refreshToken = _fixture.Create<string>();
        var refreshTokenDataModel = _fixture.Create<RefreshToken>();
        _refreshTokenRepository.Setup(r => r.FirstOrDefaultAsync(It.IsAny<RefreshTokenByUserIdAndTokenSpec>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(refreshTokenDataModel);

        await _refreshTokenService.Revoke(userId, refreshToken);

        _refreshTokenRepository.Verify(r => r.DeleteAsync(refreshTokenDataModel, It.IsAny<CancellationToken>()), Times.Once);
    }
}
