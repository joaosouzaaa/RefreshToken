using Microsoft.Extensions.Options;
using Moq;
using RefreshToken.API.DataTransferObjects.RefreshToken;
using RefreshToken.API.Entities;
using RefreshToken.API.Interfaces.Repositories;
using RefreshToken.API.Interfaces.Services;
using RefreshToken.API.Interfaces.Settings;
using RefreshToken.API.Options;
using RefreshToken.API.Services;
using System.Linq.Expressions;
using UnitTests.TestBuilders;

namespace UnitTests.ServicesTests;

public sealed class RefreshTokenServiceTests
{
    private readonly Mock<IRefreshTokenRepository> _refreshTokenRepositoryMock;
    private readonly IOptions<TokenOptions> _tokenOptions;
    private readonly Mock<IJwtService> _jwtServiceMock;
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<INotificationHandler> _notificationHandlerMock;
    private readonly RefreshTokenService _refreshTokenService;

    public RefreshTokenServiceTests()
    {
        _refreshTokenRepositoryMock = new Mock<IRefreshTokenRepository>();
        _tokenOptions = Options.Create(new TokenOptions()
        {
            Audience = "asd",
            ExpirationTimeInMinutes = 60,
            Issuer = "ksodkoask",
            Key = "ooo",
            RefreshTokenExpirationTimeInMinutes = 12
        });
        _jwtServiceMock = new Mock<IJwtService>();
        _userRepositoryMock = new Mock<IUserRepository>();
        _notificationHandlerMock = new Mock<INotificationHandler>();
        _refreshTokenService = new RefreshTokenService(
            _tokenOptions,
            _refreshTokenRepositoryMock.Object,
            _jwtServiceMock.Object,
            _userRepositoryMock.Object,
            _notificationHandlerMock.Object);
    }

    [Fact]
    public async Task AddOrReplaceRefreshTokenAsync_RefreshTokenDoesNotExist_SuccessfulScenario_CallsAdd()
    {
        // A
        const string userId = "oaksdoak";

        _refreshTokenRepositoryMock.Setup(r => r.GetByPredicateAsync(
            It.IsAny<Expression<Func<ApplicationRefreshToken, bool>>>(),
            It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult<ApplicationRefreshToken?>(null));

        // A
        var refreshTokenValueResult = await _refreshTokenService.AddOrReplaceRefreshTokenAsync(userId, default);

        // A
        _refreshTokenRepositoryMock.Verify(r => r.AddAsync(
            It.IsAny<ApplicationRefreshToken>(),
            It.IsAny<CancellationToken>()),
            Times.Once());

        _refreshTokenRepositoryMock.Verify(r => r.UpdateAsync(
            It.IsAny<ApplicationRefreshToken>(),
            It.IsAny<CancellationToken>()),
            Times.Never());
    }

    [Fact]
    public async Task AddOrReplaceRefreshTokenAsync_RefreshTokenExists_SuccessfulScenario_CallsUpdate()
    {
        // A
        const string userId = "oaksdoak";

        var refreshToken = RefreshTokenBuilder.NewObject().DomainBuild();
        _refreshTokenRepositoryMock.Setup(r => r.GetByPredicateAsync(
            It.IsAny<Expression<Func<ApplicationRefreshToken, bool>>>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(refreshToken);

        // A
        var refreshTokenValueResult = await _refreshTokenService.AddOrReplaceRefreshTokenAsync(userId, default);

        // A
        _refreshTokenRepositoryMock.Verify(r => r.AddAsync(
            It.IsAny<ApplicationRefreshToken>(),
            It.IsAny<CancellationToken>()),
            Times.Never());

        _refreshTokenRepositoryMock.Verify(r => r.UpdateAsync(
            It.IsAny<ApplicationRefreshToken>(),
            It.IsAny<CancellationToken>()),
            Times.Once());
    }

    [Fact]
    public async Task GenerateRefreshTokenAsync_SuccessfulScenario_ReturnsBearerResponse()
    {
        // A
        const string authenticationToken = "asdoka";
        var generateRefreshTokenRequest = new GenerateRefreshTokenRequest(
            authenticationToken,
            "ofkok");

        const string nameIdentifier = "oaskdok";
        _jwtServiceMock.Setup(j => j.GetNameIdentifierFromTokenAsync(
            It.IsAny<string>()))
            .ReturnsAsync(nameIdentifier);

        var user = UserBuilder.NewObject().DomainBuild();
        _userRepositoryMock.Setup(u => u.GetByPredicateAsync(
            It.IsAny<Expression<Func<User, bool>>>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _userRepositoryMock.Setup(u => u.GetAuthenticationTokenAsync(
            It.IsAny<User>()))
            .ReturnsAsync(authenticationToken);

        var refreshToken = RefreshTokenBuilder.NewObject().WithExpiryDate(DateTime.UtcNow.AddMinutes(3)).DomainBuild();
        _refreshTokenRepositoryMock.SetupSequence(r => r.GetByPredicateAsync(
            It.IsAny<Expression<Func<ApplicationRefreshToken, bool>>>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(refreshToken)
            .ReturnsAsync(refreshToken);
        const int expectedGetByPredicateCalls = 2;

        const string newAuthenticationToken = "oasdkoak";
        _jwtServiceMock.Setup(j => j.GenerateToken(
            It.IsAny<User>()))
            .Returns(newAuthenticationToken);

        // A
        var generateRefreshTokenResponseResult = await _refreshTokenService.GenerateRefreshTokenAsync(generateRefreshTokenRequest, default);

        // A
        _notificationHandlerMock.Verify(n => n.AddNotification(
            It.IsAny<string>(),
            It.IsAny<string>()),
            Times.Never());

        _jwtServiceMock.Verify(j => j.GetNameIdentifierFromTokenAsync(
            It.IsAny<string>()),
            Times.Once());

        _userRepositoryMock.Verify(u => u.GetByPredicateAsync(
           It.IsAny<Expression<Func<User, bool>>>(),
           It.IsAny<CancellationToken>()),
           Times.Once());

        _userRepositoryMock.Verify(u => u.GetAuthenticationTokenAsync(
            It.IsAny<User>()),
            Times.Once());

        _refreshTokenRepositoryMock.Verify(r => r.GetByPredicateAsync(
            It.IsAny<Expression<Func<ApplicationRefreshToken, bool>>>(),
            It.IsAny<CancellationToken>()),
            Times.Exactly(expectedGetByPredicateCalls));

        _jwtServiceMock.Verify(j => j.GenerateToken(
            It.IsAny<User>()),
            Times.Once());

        _refreshTokenRepositoryMock.Verify(r => r.UpdateAsync(
            It.IsAny<ApplicationRefreshToken>(),
            It.IsAny<CancellationToken>()),
            Times.Once());

        _userRepositoryMock.Verify(u => u.SetAuthenticationTokenAsync(
            It.IsAny<User>(),
            It.IsAny<string>()),
            Times.Once());

        Assert.NotNull(generateRefreshTokenResponseResult);
        Assert.Equal(newAuthenticationToken, generateRefreshTokenResponseResult.AccessToken);
    }

    [Fact]
    public async Task GenerateRefreshTokenAsync_NameIdentifierIsNull_ReturnsNull()
    {
        // A
        var generateRefreshTokenRequest = new GenerateRefreshTokenRequest(
            "aosdko",
            "ofkok");

        _jwtServiceMock.Setup(j => j.GetNameIdentifierFromTokenAsync(
            It.IsAny<string>()))
            .Returns(Task.FromResult<string?>(null));

        // A
        var generateRefreshTokenResponseResult = await _refreshTokenService.GenerateRefreshTokenAsync(generateRefreshTokenRequest, default);

        // A
        _notificationHandlerMock.Verify(n => n.AddNotification(
            It.IsAny<string>(),
            It.IsAny<string>()),
            Times.Once());

        _jwtServiceMock.Verify(j => j.GetNameIdentifierFromTokenAsync(
            It.IsAny<string>()),
            Times.Once());

        _userRepositoryMock.Verify(u => u.GetByPredicateAsync(
           It.IsAny<Expression<Func<User, bool>>>(),
           It.IsAny<CancellationToken>()),
           Times.Never());

        _userRepositoryMock.Verify(u => u.GetAuthenticationTokenAsync(
            It.IsAny<User>()),
            Times.Never());

        _refreshTokenRepositoryMock.Verify(r => r.GetByPredicateAsync(
            It.IsAny<Expression<Func<ApplicationRefreshToken, bool>>>(),
            It.IsAny<CancellationToken>()),
            Times.Never());

        _jwtServiceMock.Verify(j => j.GenerateToken(
            It.IsAny<User>()),
            Times.Never());

        _refreshTokenRepositoryMock.Verify(r => r.AddAsync(
            It.IsAny<ApplicationRefreshToken>(),
            It.IsAny<CancellationToken>()),
            Times.Never());

        _refreshTokenRepositoryMock.Verify(r => r.UpdateAsync(
            It.IsAny<ApplicationRefreshToken>(),
            It.IsAny<CancellationToken>()),
            Times.Never());

        _userRepositoryMock.Verify(u => u.SetAuthenticationTokenAsync(
            It.IsAny<User>(),
            It.IsAny<string>()),
            Times.Never());

        Assert.Null(generateRefreshTokenResponseResult);
    }

    [Fact]
    public async Task GenerateRefreshTokenAsync_UserDoesNotExist_ReturnsNull()
    {
        // A
        var generateRefreshTokenRequest = new GenerateRefreshTokenRequest(
            "askdoak",
            "ofkok");

        const string nameIdentifier = "oaskdok";
        _jwtServiceMock.Setup(j => j.GetNameIdentifierFromTokenAsync(
            It.IsAny<string>()))
            .ReturnsAsync(nameIdentifier);

        _userRepositoryMock.Setup(u => u.GetByPredicateAsync(
            It.IsAny<Expression<Func<User, bool>>>(),
            It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult<User?>(null));

        // A
        var generateRefreshTokenResponseResult = await _refreshTokenService.GenerateRefreshTokenAsync(generateRefreshTokenRequest, default);

        // A
        _notificationHandlerMock.Verify(n => n.AddNotification(
            It.IsAny<string>(),
            It.IsAny<string>()),
            Times.Once());

        _jwtServiceMock.Verify(j => j.GetNameIdentifierFromTokenAsync(
            It.IsAny<string>()),
            Times.Once());

        _userRepositoryMock.Verify(u => u.GetByPredicateAsync(
           It.IsAny<Expression<Func<User, bool>>>(),
           It.IsAny<CancellationToken>()),
           Times.Once());

        _userRepositoryMock.Verify(u => u.GetAuthenticationTokenAsync(
            It.IsAny<User>()),
            Times.Never());

        _refreshTokenRepositoryMock.Verify(r => r.GetByPredicateAsync(
            It.IsAny<Expression<Func<ApplicationRefreshToken, bool>>>(),
            It.IsAny<CancellationToken>()),
            Times.Never());

        _jwtServiceMock.Verify(j => j.GenerateToken(
            It.IsAny<User>()),
            Times.Never());

        _refreshTokenRepositoryMock.Verify(r => r.AddAsync(
            It.IsAny<ApplicationRefreshToken>(),
            It.IsAny<CancellationToken>()),
            Times.Never());

        _refreshTokenRepositoryMock.Verify(r => r.UpdateAsync(
            It.IsAny<ApplicationRefreshToken>(),
            It.IsAny<CancellationToken>()),
            Times.Never());

        _userRepositoryMock.Verify(u => u.SetAuthenticationTokenAsync(
            It.IsAny<User>(),
            It.IsAny<string>()),
            Times.Never());

        Assert.Null(generateRefreshTokenResponseResult);
    }

    [Fact]
    public async Task GenerateRefreshTokenAsync_RequestAuthenticationTokenDoesNotMatchAuthenticationToken_ReturnsNull()
    {
        // A
        const string requestAuthenticationToken = "asdoka";
        var generateRefreshTokenRequest = new GenerateRefreshTokenRequest(
            requestAuthenticationToken,
            "ofkok");

        const string nameIdentifier = "oaskdok";
        _jwtServiceMock.Setup(j => j.GetNameIdentifierFromTokenAsync(
            It.IsAny<string>()))
            .ReturnsAsync(nameIdentifier);

        var user = UserBuilder.NewObject().DomainBuild();
        _userRepositoryMock.Setup(u => u.GetByPredicateAsync(
            It.IsAny<Expression<Func<User, bool>>>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        const string authenticationToken = "aoskdoa";
        _userRepositoryMock.Setup(u => u.GetAuthenticationTokenAsync(
            It.IsAny<User>()))
            .ReturnsAsync(authenticationToken);

        // A
        var generateRefreshTokenResponseResult = await _refreshTokenService.GenerateRefreshTokenAsync(generateRefreshTokenRequest, default);

        // A
        _notificationHandlerMock.Verify(n => n.AddNotification(
            It.IsAny<string>(),
            It.IsAny<string>()),
            Times.Once());

        _jwtServiceMock.Verify(j => j.GetNameIdentifierFromTokenAsync(
            It.IsAny<string>()),
            Times.Once());

        _userRepositoryMock.Verify(u => u.GetByPredicateAsync(
           It.IsAny<Expression<Func<User, bool>>>(),
           It.IsAny<CancellationToken>()),
           Times.Once());

        _userRepositoryMock.Verify(u => u.GetAuthenticationTokenAsync(
            It.IsAny<User>()),
            Times.Once());

        _refreshTokenRepositoryMock.Verify(r => r.GetByPredicateAsync(
            It.IsAny<Expression<Func<ApplicationRefreshToken, bool>>>(),
            It.IsAny<CancellationToken>()),
            Times.Never());

        _jwtServiceMock.Verify(j => j.GenerateToken(
            It.IsAny<User>()),
            Times.Never());

        _refreshTokenRepositoryMock.Verify(r => r.AddAsync(
            It.IsAny<ApplicationRefreshToken>(),
            It.IsAny<CancellationToken>()),
            Times.Never());

        _refreshTokenRepositoryMock.Verify(r => r.UpdateAsync(
            It.IsAny<ApplicationRefreshToken>(),
            It.IsAny<CancellationToken>()),
            Times.Never());

        _userRepositoryMock.Verify(u => u.SetAuthenticationTokenAsync(
            It.IsAny<User>(),
            It.IsAny<string>()),
            Times.Never());

        Assert.Null(generateRefreshTokenResponseResult);
    }

    [Fact]
    public async Task GenerateRefreshTokenAsync_RefreshTokenDoesNotExist_ReturnsNull()
    {
        // A
        const string authenticationToken = "asdoka";
        var generateRefreshTokenRequest = new GenerateRefreshTokenRequest(
            authenticationToken,
            "ofkok");

        const string nameIdentifier = "oaskdok";
        _jwtServiceMock.Setup(j => j.GetNameIdentifierFromTokenAsync(
            It.IsAny<string>()))
            .ReturnsAsync(nameIdentifier);

        var user = UserBuilder.NewObject().DomainBuild();
        _userRepositoryMock.Setup(u => u.GetByPredicateAsync(
            It.IsAny<Expression<Func<User, bool>>>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _userRepositoryMock.Setup(u => u.GetAuthenticationTokenAsync(
            It.IsAny<User>()))
            .ReturnsAsync(authenticationToken);

        _refreshTokenRepositoryMock.Setup(r => r.GetByPredicateAsync(
            It.IsAny<Expression<Func<ApplicationRefreshToken, bool>>>(),
            It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult<ApplicationRefreshToken?>(null));

        // A
        var generateRefreshTokenResponseResult = await _refreshTokenService.GenerateRefreshTokenAsync(generateRefreshTokenRequest, default);

        // A
        _notificationHandlerMock.Verify(n => n.AddNotification(
            It.IsAny<string>(),
            It.IsAny<string>()),
            Times.Once());

        _jwtServiceMock.Verify(j => j.GetNameIdentifierFromTokenAsync(
            It.IsAny<string>()),
            Times.Once());

        _userRepositoryMock.Verify(u => u.GetByPredicateAsync(
           It.IsAny<Expression<Func<User, bool>>>(),
           It.IsAny<CancellationToken>()),
           Times.Once());

        _userRepositoryMock.Verify(u => u.GetAuthenticationTokenAsync(
            It.IsAny<User>()),
            Times.Once());

        _refreshTokenRepositoryMock.Verify(r => r.GetByPredicateAsync(
            It.IsAny<Expression<Func<ApplicationRefreshToken, bool>>>(),
            It.IsAny<CancellationToken>()),
            Times.Once());

        _jwtServiceMock.Verify(j => j.GenerateToken(
            It.IsAny<User>()),
            Times.Never());

        _refreshTokenRepositoryMock.Verify(r => r.AddAsync(
            It.IsAny<ApplicationRefreshToken>(),
            It.IsAny<CancellationToken>()),
            Times.Never());

        _refreshTokenRepositoryMock.Verify(r => r.UpdateAsync(
            It.IsAny<ApplicationRefreshToken>(),
            It.IsAny<CancellationToken>()),
            Times.Never());

        _userRepositoryMock.Verify(u => u.SetAuthenticationTokenAsync(
            It.IsAny<User>(),
            It.IsAny<string>()),
            Times.Never());

        Assert.Null(generateRefreshTokenResponseResult);
    }

    [Fact]
    public async Task GenerateRefreshTokenAsync_ExpiryDateLessThanCurrentDate_ReturnsNull()
    {
        // A
        const string authenticationToken = "asdoka";
        var generateRefreshTokenRequest = new GenerateRefreshTokenRequest(
            authenticationToken,
            "ofkok");

        const string nameIdentifier = "oaskdok";
        _jwtServiceMock.Setup(j => j.GetNameIdentifierFromTokenAsync(
            It.IsAny<string>()))
            .ReturnsAsync(nameIdentifier);

        var user = UserBuilder.NewObject().DomainBuild();
        _userRepositoryMock.Setup(u => u.GetByPredicateAsync(
            It.IsAny<Expression<Func<User, bool>>>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _userRepositoryMock.Setup(u => u.GetAuthenticationTokenAsync(
            It.IsAny<User>()))
            .ReturnsAsync(authenticationToken);

        var refreshToken = RefreshTokenBuilder.NewObject().WithExpiryDate(DateTime.UtcNow.AddMinutes(-3)).DomainBuild();
        _refreshTokenRepositoryMock.Setup(r => r.GetByPredicateAsync(
            It.IsAny<Expression<Func<ApplicationRefreshToken, bool>>>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(refreshToken);

        // A
        var generateRefreshTokenResponseResult = await _refreshTokenService.GenerateRefreshTokenAsync(generateRefreshTokenRequest, default);

        // A
        _notificationHandlerMock.Verify(n => n.AddNotification(
            It.IsAny<string>(),
            It.IsAny<string>()),
            Times.Once());

        _jwtServiceMock.Verify(j => j.GetNameIdentifierFromTokenAsync(
            It.IsAny<string>()),
            Times.Once());

        _userRepositoryMock.Verify(u => u.GetByPredicateAsync(
           It.IsAny<Expression<Func<User, bool>>>(),
           It.IsAny<CancellationToken>()),
           Times.Once());

        _userRepositoryMock.Verify(u => u.GetAuthenticationTokenAsync(
            It.IsAny<User>()),
            Times.Once());

        _refreshTokenRepositoryMock.Verify(r => r.GetByPredicateAsync(
            It.IsAny<Expression<Func<ApplicationRefreshToken, bool>>>(),
            It.IsAny<CancellationToken>()),
            Times.Once());

        _jwtServiceMock.Verify(j => j.GenerateToken(
            It.IsAny<User>()),
            Times.Never());

        _refreshTokenRepositoryMock.Verify(r => r.AddAsync(
            It.IsAny<ApplicationRefreshToken>(),
            It.IsAny<CancellationToken>()),
            Times.Never());

        _refreshTokenRepositoryMock.Verify(r => r.UpdateAsync(
            It.IsAny<ApplicationRefreshToken>(),
            It.IsAny<CancellationToken>()),
            Times.Never());

        _userRepositoryMock.Verify(u => u.SetAuthenticationTokenAsync(
            It.IsAny<User>(),
            It.IsAny<string>()),
            Times.Never());

        Assert.Null(generateRefreshTokenResponseResult);
    }
}
