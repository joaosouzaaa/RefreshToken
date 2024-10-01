using FluentValidation;
using FluentValidation.Results;
using Moq;
using RefreshTokenAuthentication.API.Arguments;
using RefreshTokenAuthentication.API.DataTransferObjects.RefreshToken;
using RefreshTokenAuthentication.API.DataTransferObjects.User;
using RefreshTokenAuthentication.API.Entities;
using RefreshTokenAuthentication.API.Interfaces.Mappers;
using RefreshTokenAuthentication.API.Interfaces.Repositories;
using RefreshTokenAuthentication.API.Interfaces.Services;
using RefreshTokenAuthentication.API.Interfaces.Settings;
using RefreshTokenAuthentication.API.Services;
using System.Linq.Expressions;
using UnitTests.TestBuilders;

namespace UnitTests.ServicesTests;

public sealed class UserServiceTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IUserMapper> _userMapperMock;
    private readonly Mock<IJwtService> _jwtServiceMock;
    private readonly Mock<IRefreshTokenService> _refreshTokenServiceMock;
    private readonly Mock<IValidator<User>> _validatorMock;
    private readonly Mock<INotificationHandler> _notificationHandlerMock;
    private readonly UserService _userService;

    public UserServiceTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _userMapperMock = new Mock<IUserMapper>();
        _jwtServiceMock = new Mock<IJwtService>();
        _refreshTokenServiceMock = new Mock<IRefreshTokenService>();
        _validatorMock = new Mock<IValidator<User>>();
        _notificationHandlerMock = new Mock<INotificationHandler>();
        _userService = new UserService(
            _userRepositoryMock.Object,
            _userMapperMock.Object,
            _jwtServiceMock.Object,
            _refreshTokenServiceMock.Object,
            _validatorMock.Object,
            _notificationHandlerMock.Object);
    }

    [Fact]
    public async Task CreateAsync_SuccessfulScenario_CallsCreateAsync()
    {
        // A
        var createRequest = UserBuilder.NewObject().CreateRequestBuild();

        _userRepositoryMock.Setup(u => u.UserNameExistsAsync(
            It.IsAny<string>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var user = UserBuilder.NewObject().DomainBuild();
        _userMapperMock.Setup(u => u.CreateRequestToDomain(
            It.IsAny<CreateUserRequest>()))
            .Returns(user);

        var validationResult = new ValidationResult();
        _validatorMock.Setup(v => v.ValidateAsync(
            It.IsAny<User>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(validationResult);

        // A
        await _userService.CreateAsync(createRequest, default);

        // A
        _notificationHandlerMock.Verify(n => n.AddNotification(
            It.IsAny<string>(),
            It.IsAny<string>()),
            Times.Never());

        _userRepositoryMock.Verify(u => u.UserNameExistsAsync(
           It.IsAny<string>(),
           It.IsAny<CancellationToken>()),
           Times.Once());

        _userMapperMock.Verify(u => u.CreateRequestToDomain(
            It.IsAny<CreateUserRequest>()),
            Times.Once());

        _validatorMock.Verify(v => v.ValidateAsync(
            It.IsAny<User>(),
            It.IsAny<CancellationToken>()),
            Times.Once());

        _userRepositoryMock.Verify(u => u.CreateAsync(
            It.IsAny<User>()),
            Times.Once());
    }

    [Fact]
    public async Task CreateAsync_UserNameExists_CallsAddNotification()
    {
        // A
        var createRequest = UserBuilder.NewObject().CreateRequestBuild();

        _userRepositoryMock.Setup(u => u.UserNameExistsAsync(
            It.IsAny<string>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // A
        await _userService.CreateAsync(createRequest, default);

        // A
        _notificationHandlerMock.Verify(n => n.AddNotification(
            It.IsAny<string>(),
            It.IsAny<string>()),
            Times.Once());

        _userRepositoryMock.Verify(u => u.UserNameExistsAsync(
           It.IsAny<string>(),
           It.IsAny<CancellationToken>()),
           Times.Once());

        _userMapperMock.Verify(u => u.CreateRequestToDomain(
            It.IsAny<CreateUserRequest>()),
            Times.Never());

        _validatorMock.Verify(v => v.ValidateAsync(
            It.IsAny<User>(),
            It.IsAny<CancellationToken>()),
            Times.Never());

        _userRepositoryMock.Verify(u => u.CreateAsync(
            It.IsAny<User>()),
            Times.Never());
    }

    [Fact]
    public async Task CreateAsync_InvalidUser_CallsAddNotification()
    {
        // A
        var createRequest = UserBuilder.NewObject().CreateRequestBuild();

        _userRepositoryMock.Setup(u => u.UserNameExistsAsync(
            It.IsAny<string>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var user = UserBuilder.NewObject().DomainBuild();
        _userMapperMock.Setup(u => u.CreateRequestToDomain(
            It.IsAny<CreateUserRequest>()))
            .Returns(user);

        var validationFailureList = new List<ValidationFailure>()
        {
            new("dasd", "asdasd"),
            new("dasd", "asdasd"),
            new("dasd", "asdasd")
        };
        var validationResult = new ValidationResult(validationFailureList);
        _validatorMock.Setup(v => v.ValidateAsync(
            It.IsAny<User>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(validationResult);

        // A
        await _userService.CreateAsync(createRequest, default);

        // A
        _notificationHandlerMock.Verify(n => n.AddNotification(
            It.IsAny<string>(),
            It.IsAny<string>()),
            Times.Exactly(validationResult.Errors.Count));

        _userRepositoryMock.Verify(u => u.UserNameExistsAsync(
           It.IsAny<string>(),
           It.IsAny<CancellationToken>()),
           Times.Once());

        _userMapperMock.Verify(u => u.CreateRequestToDomain(
            It.IsAny<CreateUserRequest>()),
            Times.Once());

        _validatorMock.Verify(v => v.ValidateAsync(
            It.IsAny<User>(),
            It.IsAny<CancellationToken>()),
            Times.Once());

        _userRepositoryMock.Verify(u => u.CreateAsync(
            It.IsAny<User>()),
            Times.Never());
    }

    [Fact]
    public async Task GetByIdAsync_SuccessfulScenario_ReturnsResponseResult()
    {
        // A
        const string id = "test";

        var user = UserBuilder.NewObject().DomainBuild();
        _userRepositoryMock.Setup(u => u.GetByPredicateAsync(
            It.IsAny<Expression<Func<User, bool>>>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var getByIdResponse = UserBuilder.NewObject().GetByIdResponseBuild();
        _userMapperMock.Setup(u => u.DomainToGetByIdResponse(
            It.IsAny<User>()))
            .Returns(getByIdResponse);

        // A
        var getByIdResponseResult = await _userService.GetByIdAsync(id, default);

        // A
        _userRepositoryMock.Verify(u => u.GetByPredicateAsync(
           It.IsAny<Expression<Func<User, bool>>>(),
           It.IsAny<CancellationToken>()),
           Times.Once());

        _userMapperMock.Verify(u => u.DomainToGetByIdResponse(
            It.IsAny<User>()),
            Times.Once());

        Assert.NotNull(getByIdResponseResult);
    }

    [Fact]
    public async Task GetByIdAsync_UserDoesNotExist_ReturnsNull()
    {
        // A
        const string id = "test";

        _userRepositoryMock.Setup(u => u.GetByPredicateAsync(
            It.IsAny<Expression<Func<User, bool>>>(),
            It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult<User?>(null));

        // A
        var getByIdResponseResult = await _userService.GetByIdAsync(id, default);

        // A
        _userRepositoryMock.Verify(u => u.GetByPredicateAsync(
           It.IsAny<Expression<Func<User, bool>>>(),
           It.IsAny<CancellationToken>()),
           Times.Once());

        _userMapperMock.Verify(u => u.DomainToGetByIdResponse(
            It.IsAny<User>()),
            Times.Never());

        Assert.Null(getByIdResponseResult);
    }

    [Fact]
    public async Task LoginAsync_SuccessfulScenario_ReturnsToken()
    {
        // A
        var loginRequest = UserBuilder.NewObject().LoginRequestBuild();

        var loginArgument = UserBuilder.NewObject().LoginArgumentBuild();
        _userMapperMock.Setup(u => u.LoginRequestToDomain(
            It.IsAny<LoginRequest>()))
            .Returns(loginArgument);

        _userRepositoryMock.Setup(u => u.LoginAsync(
            It.IsAny<LoginArgument>()))
            .ReturnsAsync(true);

        var user = UserBuilder.NewObject().DomainBuild();
        _userRepositoryMock.Setup(u => u.GetByPredicateAsync(
            It.IsAny<Expression<Func<User, bool>>>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        const string token = "test";
        _jwtServiceMock.Setup(j => j.GenerateToken(
            It.IsAny<User>()))
            .Returns(token);

        var addOrUpdateRefreshTokenResponse = new AddOrUpdateRefreshTokenResponse(
            "test",
            DateTime.UtcNow);
        _refreshTokenServiceMock.Setup(r => r.AddOrReplaceRefreshTokenAsync(
            It.IsAny<string>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(addOrUpdateRefreshTokenResponse);

        // A
        var bearerTokenResponseResult = await _userService.LoginAsync(loginRequest, default);

        // A
        _notificationHandlerMock.Verify(n => n.AddNotification(
           It.IsAny<string>(),
           It.IsAny<string>()),
           Times.Never());

        _userMapperMock.Verify(u => u.LoginRequestToDomain(
            It.IsAny<LoginRequest>()),
            Times.Once());

        _userRepositoryMock.Verify(u => u.LoginAsync(
            It.IsAny<LoginArgument>()),
            Times.Once());

        _userRepositoryMock.Verify(u => u.GetByPredicateAsync(
            It.IsAny<Expression<Func<User, bool>>>(),
            It.IsAny<CancellationToken>()),
            Times.Once());

        _jwtServiceMock.Verify(j => j.GenerateToken(
            It.IsAny<User>()),
            Times.Once());

        _userRepositoryMock.Verify(u => u.SetAuthenticationTokenAsync(
            It.IsAny<User>(),
            It.IsAny<string>()),
            Times.Once());

        _refreshTokenServiceMock.Verify(r => r.AddOrReplaceRefreshTokenAsync(
           It.IsAny<string>(),
           It.IsAny<CancellationToken>()),
           Times.Once());

        Assert.NotNull(bearerTokenResponseResult);
        Assert.Equal(token, bearerTokenResponseResult.AccessToken);
        Assert.Equal(addOrUpdateRefreshTokenResponse.Value, bearerTokenResponseResult.RefreshToken);
        Assert.Equal(addOrUpdateRefreshTokenResponse.ExpiryDate, bearerTokenResponseResult.RefreshTokenExpiryDate);
    }

    [Fact]
    public async Task LoginAsync_InvalidLogin_ReturnsNull()
    {
        // A
        var loginRequest = UserBuilder.NewObject().LoginRequestBuild();

        var loginArgument = UserBuilder.NewObject().LoginArgumentBuild();
        _userMapperMock.Setup(u => u.LoginRequestToDomain(
            It.IsAny<LoginRequest>()))
            .Returns(loginArgument);

        _userRepositoryMock.Setup(u => u.LoginAsync(
            It.IsAny<LoginArgument>()))
            .ReturnsAsync(false);

        // A
        var bearerTokenResponseResult = await _userService.LoginAsync(loginRequest, default);

        // A
        _notificationHandlerMock.Verify(n => n.AddNotification(
           It.IsAny<string>(),
           It.IsAny<string>()),
           Times.Once());

        _userMapperMock.Verify(u => u.LoginRequestToDomain(
            It.IsAny<LoginRequest>()),
            Times.Once());

        _userRepositoryMock.Verify(u => u.LoginAsync(
            It.IsAny<LoginArgument>()),
            Times.Once());

        _userRepositoryMock.Verify(u => u.GetByPredicateAsync(
            It.IsAny<Expression<Func<User, bool>>>(),
            It.IsAny<CancellationToken>()),
            Times.Never());

        _jwtServiceMock.Verify(j => j.GenerateToken(
            It.IsAny<User>()),
            Times.Never());

        _userRepositoryMock.Verify(u => u.SetAuthenticationTokenAsync(
            It.IsAny<User>(),
            It.IsAny<string>()),
            Times.Never());

        _refreshTokenServiceMock.Verify(r => r.AddOrReplaceRefreshTokenAsync(
           It.IsAny<string>(),
           It.IsAny<CancellationToken>()),
           Times.Never());

        Assert.Null(bearerTokenResponseResult);
    }

    [Fact]
    public async Task SignOutAsync_SuccessfulScenario_CallsRemoveAuthenticationToken()
    {
        // A
        const string userId = "asdasd";

        var user = UserBuilder.NewObject().DomainBuild();
        _userRepositoryMock.Setup(u => u.GetByPredicateAsync(
            It.IsAny<Expression<Func<User, bool>>>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        // A
        await _userService.SignOutAsync(userId, default);

        // A
        _notificationHandlerMock.Verify(n => n.AddNotification(
            It.IsAny<string>(),
            It.IsAny<string>()),
            Times.Never());

        _userRepositoryMock.Verify(u => u.RemoveAuthenticationTokenAsync(
            It.IsAny<User>()),
            Times.Once());
    }

    [Fact]
    public async Task SignOutAsync_UserDoesNotExist_CallsRemoveAuthenticationToken()
    {
        // A
        const string userId = "asdasd";

        _userRepositoryMock.Setup(u => u.GetByPredicateAsync(
            It.IsAny<Expression<Func<User, bool>>>(),
            It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult<User?>(null));

        // A
        await _userService.SignOutAsync(userId, default);

        // A
        _notificationHandlerMock.Verify(n => n.AddNotification(
            It.IsAny<string>(),
            It.IsAny<string>()),
            Times.Once());

        _userRepositoryMock.Verify(u => u.RemoveAuthenticationTokenAsync(
            It.IsAny<User>()),
            Times.Never());
    }
}
