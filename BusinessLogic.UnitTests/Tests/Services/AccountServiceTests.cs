using BusinessLogic.Abstractions;
using BusinessLogic.Dtos;
using BusinessLogic.Options;
using BusinessLogic.Services;
using BusinessLogic.UnitTests.Mappers;
using DataAccess.Abstractions;
using DataAccess.Entities;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace BusinessLogic.UnitTests.Tests.Services;
public class AccountServiceTests
{
    private readonly Mock<IJwtTokenProvider> _jwtTokenProvider = new();
    private readonly Mock<UserManager<User>> _userManager;
    private readonly Mock<IValidator<RegisterAccountDto>> _registerValidator = new();
    private readonly Mock<IRepository<RefreshToken>> _refreshTokenRepository = new();
    private readonly Mock<IOptions<TokenProvidersOptions>> _tokenProvidersOptions = new();
    private readonly Mock<IRepository<User>> _userRepository = new();
    private readonly Mock<IValidator<UpdateUserDto>> _updateUserValidator = new();
    private readonly Mock<IBlobRepository> _blobRepository = new();
    private readonly Mock<TimeProvider> _timeProvider = new();

    private readonly AccountService _accountService;

    public AccountServiceTests()
    {
        _userManager = new Mock<UserManager<User>>(Mock.Of<IUserStore<User>>(), null!, null!, null!, null!, null!, null!, null!, null!);

        _accountService = new(
            _userManager.Object,
            _registerValidator.Object,
            _jwtTokenProvider.Object,
            _refreshTokenRepository.Object,
            _tokenProvidersOptions.Object,
            _userRepository.Object,
            _updateUserValidator.Object,
            TestMapper.Mapper,
            _blobRepository.Object,
            _timeProvider.Object);
    }

    [Fact]
    public void Test1()
    {
        var mapper = TestMapper.Mapper;
    }
}
