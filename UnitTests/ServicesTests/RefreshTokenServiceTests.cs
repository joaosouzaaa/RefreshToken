using Microsoft.Extensions.Options;
using Moq;
using RefreshToken.API.Entities;
using RefreshToken.API.Interfaces.Repositories;
using RefreshToken.API.Options;
using RefreshToken.API.Services;
using System.Linq.Expressions;
using UnitTests.TestBuilders;

namespace UnitTests.ServicesTests;

public sealed class RefreshTokenServiceTests
{
    private readonly Mock<IRefreshTokenRepository> _refreshTokenRepositoryMock;
    private readonly IOptions<TokenOptions> _tokenOptions;
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
        _refreshTokenService = new RefreshTokenService(
            _tokenOptions,
            _refreshTokenRepositoryMock.Object);
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
}