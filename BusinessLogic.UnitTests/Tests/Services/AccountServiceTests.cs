using AutoFixture;
using BusinessLogic.Abstractions;
using BusinessLogic.Constants;
using BusinessLogic.Dtos;
using BusinessLogic.Exceptions;
using BusinessLogic.Options;
using BusinessLogic.Services;
using BusinessLogic.UnitTests.Customizations;
using BusinessLogic.UnitTests.Mappers;
using DataAccess.Abstractions;
using DataAccess.Entities;
using DataAccess.Specifications;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Moq;
using System.Security.Claims;
using Xunit;

namespace BusinessLogic.UnitTests.Tests.Services;
public class AccountServiceTests
{
    private readonly Mock<IJwtTokenProvider> _jwtTokenProvider = new();
    private readonly Mock<UserManager<User>> _userManager;
    private readonly Mock<IValidator<RegisterAccountDto>> _registerValidator = new();
    private readonly Mock<IRepository<RefreshToken>> _refreshTokenRepository = new();
    private readonly IOptions<TokenProvidersOptions> _tokenProvidersOptions;
    private readonly Mock<IRepository<User>> _userRepository = new();
    private readonly Mock<IValidator<UpdateUserDto>> _updateUserValidator = new();
    private readonly Mock<IBlobRepository> _blobRepository = new();
    private readonly Mock<TimeProvider> _timeProvider = new();

    private readonly AccountService _accountService;

    private readonly Fixture _fixture = new();

    public AccountServiceTests()
    {
        _fixture.Customize(new DomainCustomization());

        _userManager = new Mock<UserManager<User>>(Mock.Of<IUserStore<User>>(), null!, null!, null!, null!, null!, null!, null!, null!);
        var tokenProvidersOptionsMock = new Mock<IOptions<TokenProvidersOptions>>();
        tokenProvidersOptionsMock.Setup(o => o.Value)
            .Returns(_fixture.Create<TokenProvidersOptions>());
        _tokenProvidersOptions = tokenProvidersOptionsMock.Object;

        _accountService = new(
            _userManager.Object,
            _registerValidator.Object,
            _jwtTokenProvider.Object,
            _refreshTokenRepository.Object,
            _tokenProvidersOptions,
            _userRepository.Object,
            _updateUserValidator.Object,
            TestMapper.Mapper,
            _blobRepository.Object,
            _timeProvider.Object);
    }

    [Fact]
    public async Task Login_ReturnsNotSuccessfulResult_When_EmailOrPasswordIsWrong()
    {
        var dto = _fixture.Create<LoginAccountDto>();
        var user = _fixture.Create<User>();
        _userManager.Setup(m => m.FindByNameAsync(dto.Email))
            .ReturnsAsync(user);
        _userManager.Setup(m => m.CheckPasswordAsync(user, dto.Password))
            .ReturnsAsync(false);

        var result = await _accountService.Login(dto);

        result.IsSuccessful.Should().BeFalse();
        result.ErrorMessage.Should().Be("Wrong email or password");
    }

    [Fact]
    public async Task Login_SavesRefreshToken_When_Success()
    {
        var dto = _fixture.Create<LoginAccountDto>();
        var user = _fixture.Create<User>();
        var expectedRefreshToken = _fixture.Create<string>();
        var currentTime = _fixture.Create<DateTimeOffset>();
        var expectedExpiryTime = currentTime.AddDays(_tokenProvidersOptions.Value.RefreshTokenLifetimeInDays);
        _userManager.Setup(m => m.FindByNameAsync(dto.Email))
            .ReturnsAsync(user);
        _userManager.Setup(m => m.CheckPasswordAsync(user, dto.Password))
            .ReturnsAsync(true);
        _jwtTokenProvider.Setup(t => t.GenerateRefreshToken())
            .Returns(expectedRefreshToken);
        _userManager.Setup(m => m.GetRolesAsync(user))
            .ReturnsAsync(_fixture.CreateMany<string>().ToList());
        _timeProvider.Setup(p => p.GetUtcNow())
            .Returns(currentTime);

        _ = await _accountService.Login(dto);

        _refreshTokenRepository.Verify(r => r.AddAsync(It.Is<RefreshToken>(t => 
                t.UserId == user.Id 
                && t.Token == expectedRefreshToken && t.ExpiryTime == expectedExpiryTime), 
                It.IsAny<CancellationToken>()), 
            Times.Once);
    }

    [Fact]
    public async Task Login_ReturnsTokensDto_When_Success()
    {
        var dto = _fixture.Create<LoginAccountDto>();
        var user = _fixture.Create<User>();
        var expectedTokens = _fixture.Create<TokensDto>();
        var currentTime = _fixture.Create<DateTimeOffset>();
        _userManager.Setup(m => m.FindByNameAsync(dto.Email))
            .ReturnsAsync(user);
        _userManager.Setup(m => m.CheckPasswordAsync(user, dto.Password))
            .ReturnsAsync(true);
        _jwtTokenProvider.Setup(t => t.GenerateRefreshToken())
            .Returns(expectedTokens.RefreshToken);
        _jwtTokenProvider.Setup(t => t.GenerateAccessToken(It.IsAny<IEnumerable<Claim>>()))
            .Returns(expectedTokens.AccessToken);
        _userManager.Setup(m => m.GetRolesAsync(user))
            .ReturnsAsync(_fixture.CreateMany<string>().ToList());
        _timeProvider.Setup(p => p.GetUtcNow())
            .Returns(currentTime);

        var result = await _accountService.Login(dto);

        result.IsSuccessful.Should().BeTrue();
        result.Tokens.Should().Be(expectedTokens);
    }

    [Fact]
    public async Task Register_ThrowsEntityValidationFailedException_When_DtoIsInvalid()
    {
        var dto = _fixture.Create<RegisterAccountDto>();
        var validationResult = _fixture.Create<ValidationResult>();
        _registerValidator.Setup(v => v.Validate(dto))
            .Returns(validationResult);
        var expectedException = new EntityValidationFailedException(validationResult.Errors);

        var act = () => _accountService.Register(dto);

        await act.Should().ThrowAsync<EntityValidationFailedException>()
            .WithMessage(expectedException.Message);
    }

    [Fact]
    public async Task Register_ThrowsEntityValidationFailedException_When_UserManagerFailed()
    {
        var dto = _fixture.Create<RegisterAccountDto>();
        _registerValidator.Setup(v => v.Validate(dto))
            .Returns(new ValidationResult());
        var userManagerResult = IdentityResult.Failed(_fixture.CreateMany<IdentityError>().ToArray());
        _userManager.Setup(m => m.CreateAsync(It.IsAny<User>(), It.IsAny<string>()))
            .ReturnsAsync(userManagerResult);
        _timeProvider.Setup(p => p.GetUtcNow())
            .Returns(_fixture.Create<DateTimeOffset>());
        var expectedException = new EntityValidationFailedException(userManagerResult.Errors);

        var act = () => _accountService.Register(dto);

        await act.Should().ThrowAsync<EntityValidationFailedException>()
            .WithMessage(expectedException.Message);
    }

    [Fact]
    public async Task Register_SavesUser_When_Success()
    {
        var dto = _fixture.Create<RegisterAccountDto>();
        _registerValidator.Setup(v => v.Validate(dto))
            .Returns(new ValidationResult());
        _userManager.Setup(m => m.CreateAsync(It.IsAny<User>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);
        var expectedCreationTime = _fixture.Create<DateTimeOffset>();
        _timeProvider.Setup(p => p.GetUtcNow())
            .Returns(expectedCreationTime);

        _ = await _accountService.Register(dto);

        _userManager.Verify(m => 
            m.CreateAsync(
                It.Is<User>(u => u.Name == dto.Name && u.Email == dto.Email && u.CreatedAt == expectedCreationTime), 
                dto.Password), 
            Times.Once);
        _userManager.Verify(m => m.AddToRoleAsync(It.IsAny<User>(), RolesConstants.RegisteredViewer), Times.Once);
    }

    [Fact]
    public async Task Register_ReturnsDto_When_Success()
    {
        var dto = _fixture.Create<RegisterAccountDto>();
        _registerValidator.Setup(v => v.Validate(dto))
            .Returns(new ValidationResult());
        _userManager.Setup(m => m.CreateAsync(It.IsAny<User>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);
        _timeProvider.Setup(p => p.GetUtcNow())
            .Returns(_fixture.Create<DateTimeOffset>());

        var result = await _accountService.Register(dto);

        result.Should().BeEquivalentTo(dto, opt => opt.ExcludingMissingMembers());
    }

    [Fact]
    public async Task GetDetails_ThrowsEntityNotFoundException_When_UserWasNotFound()
    {
        var userId = _fixture.Create<Guid>();
        _userRepository.Setup(r => r.FirstOrDefaultAsync(It.IsAny<UserByIdNoTrackingSpec>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(null as User);

        var act = () => _accountService.GetDetails(userId);

        await act.Should().ThrowAsync<EntityNotFoundException>()
            .WithMessage("User was not found");
    }

    [Fact]
    public async Task GetDetails_ReturnsDto_When_Success()
    {
        var userId = _fixture.Create<Guid>();
        var user = _fixture.Create<User>();
        _userRepository.Setup(r => r.FirstOrDefaultAsync(It.IsAny<UserByIdNoTrackingSpec>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var result = await _accountService.GetDetails(userId);

        result.Should().BeEquivalentTo(user, opt => opt.ExcludingMissingMembers());
    }

    [Fact]
    public async Task UpdateDetails_ThrowsEntityValidationFailedException_When_DtoIsInvalid()
    {
        var userId = _fixture.Create<Guid>();
        var dto = _fixture.Create<UpdateUserDto>();
        var validationResult = _fixture.Create<ValidationResult>();
        _updateUserValidator.Setup(v => v.Validate(dto))
            .Returns(validationResult);
        var expectedException = new EntityValidationFailedException(validationResult.Errors);

        var act = () => _accountService.UpdateDetails(userId, dto);

        await act.Should().ThrowAsync<EntityValidationFailedException>()
            .WithMessage(expectedException.Message);
    }

    [Fact]
    public async Task UpdateDetails_ThrowsEntityNotFoundException_When_UserWasNotFound()
    {
        var userId = _fixture.Create<Guid>();
        var dto = _fixture.Create<UpdateUserDto>();
        _updateUserValidator.Setup(v => v.Validate(dto))
            .Returns(new ValidationResult());
        _userRepository.Setup(r => r.FirstOrDefaultAsync(It.IsAny<UserByIdSpec>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(null as User);

        var act = () => _accountService.UpdateDetails(userId, dto);

        await act.Should().ThrowAsync<EntityNotFoundException>()
            .WithMessage("User was not found");
    }

    [Fact]
    public async Task UpdateDetails_ThrowsEntityValidationFailedException_When_AvatarWasNotFound()
    {
        var user = _fixture.Create<User>();
        var dto = _fixture.Create<UpdateUserDto>();
        _updateUserValidator.Setup(v => v.Validate(dto))
            .Returns(new ValidationResult());
        _userRepository.Setup(r => r.FirstOrDefaultAsync(It.IsAny<UserByIdSpec>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _blobRepository.Setup(b => b.ExistsAsync(dto.AvatarLocalFileName!))
            .ReturnsAsync(false);

        var act = () => _accountService.UpdateDetails(user.Id, dto);

        await act.Should().ThrowAsync<EntityValidationFailedException>()
            .WithMessage($"Image with name {dto.AvatarLocalFileName} was not found");
    }

    [Fact]
    public async Task UpdateDetails_UpdatesUserWithoutBlog_When_SuccessAndUserHasNotBlog()
    {
        var user = _fixture.Build<User>()
            .Without(u => u.Blog)
            .Create();
        var dto = _fixture.Create<UpdateUserDto>();
        _updateUserValidator.Setup(v => v.Validate(dto))
            .Returns(new ValidationResult());
        _userRepository.Setup(r => r.FirstOrDefaultAsync(It.IsAny<UserByIdSpec>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _blobRepository.Setup(b => b.ExistsAsync(dto.AvatarLocalFileName!))
            .ReturnsAsync(true);

        _ = await _accountService.UpdateDetails(user.Id, dto);

        _userRepository.Verify(
            r => r.UpdateAsync(
                It.Is<User>(u => u.Name == dto.Name
                    && u.AvatarLocalFileName == dto.AvatarLocalFileName), 
                It.IsAny<CancellationToken>()), 
            Times.Once);
    }

    [Fact]
    public async Task UpdateDetails_UpdatesUserAndBlog_When_SuccessAndUserHasBlog()
    {
        var user = _fixture.Create<User>();
        var dto = _fixture.Create<UpdateUserDto>();
        _updateUserValidator.Setup(v => v.Validate(dto))
            .Returns(new ValidationResult());
        _userRepository.Setup(r => r.FirstOrDefaultAsync(It.IsAny<UserByIdSpec>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _blobRepository.Setup(b => b.ExistsAsync(dto.AvatarLocalFileName!))
            .ReturnsAsync(true);

        _ = await _accountService.UpdateDetails(user.Id, dto);

        _userRepository.Verify(
            r => r.UpdateAsync(
                It.Is<User>(u => u.Name == dto.Name
                    && u.AvatarLocalFileName == dto.AvatarLocalFileName
                    && u.Blog!.Name == dto.Name
                    && u.Blog.AvatarLocalFileName == dto.AvatarLocalFileName),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task UpdateDetails_ReturnsDto_When_Success()
    {
        var user = _fixture.Build<User>()
            .Without(u => u.Blog)
            .Create();
        var dto = _fixture.Create<UpdateUserDto>();
        _updateUserValidator.Setup(v => v.Validate(dto))
            .Returns(new ValidationResult());
        _userRepository.Setup(r => r.FirstOrDefaultAsync(It.IsAny<UserByIdSpec>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _blobRepository.Setup(b => b.ExistsAsync(dto.AvatarLocalFileName!))
            .ReturnsAsync(true);

        var result = await _accountService.UpdateDetails(user.Id, dto);

        result.Should().BeEquivalentTo(dto, opt => opt.ExcludingMissingMembers());
    }
}
