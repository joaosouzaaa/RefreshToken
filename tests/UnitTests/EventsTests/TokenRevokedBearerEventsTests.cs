using Microsoft.AspNetCore.Authentication.JwtBearer;
using Moq;
using RefreshTokenAuthentication.API.Entities;
using RefreshTokenAuthentication.API.Events;
using RefreshTokenAuthentication.API.Interfaces.Repositories;
using System.IdentityModel.Tokens.Jwt;
using System.Linq.Expressions;
using UnitTests.TestBuilders;

namespace UnitTests.EventsTests;

public sealed class TokenRevokedBearerEventsTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly TokenRevokedBearerEvents _events;
    private const string _validTokenValue = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ.SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c";

    public TokenRevokedBearerEventsTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _events = new TokenRevokedBearerEvents(_userRepositoryMock.Object);
    }

    [Fact]
    public async Task TokenValidated_SuccessfulScenario_DoesNotCallFail()
    {
        // A
        var token = new JwtSecurityToken(_validTokenValue);
        var tokenValidatedContext = TokenValidatedContextBuilder.BuildTokenValidatedContext(token);

        var user = UserBuilder.NewObject().DomainBuild();
        _userRepositoryMock.Setup(u => u.GetByPredicateAsync(
            It.IsAny<Expression<Func<User, bool>>>(), 
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _userRepositoryMock.Setup(u => u.GetAuthenticationTokenAsync(
            It.IsAny<User>()))
            .ReturnsAsync(_validTokenValue);

        // A
        await _events.TokenValidated(tokenValidatedContext);

        // A
        _userRepositoryMock.Verify(u => u.GetByPredicateAsync(
            It.IsAny<Expression<Func<User, bool>>>(),
            It.IsAny<CancellationToken>()),
            Times.Once());

        _userRepositoryMock.Verify(u => u.GetAuthenticationTokenAsync(
            It.IsAny<User>()),
            Times.Once());

        Assert.Null(tokenValidatedContext.Result);
    }

    [Fact]
    public async Task TokenValidated_TokenIsNull_ReturnCall()
    {
        // A
        var tokenValidatedContext = TokenValidatedContextBuilder.BuildTokenValidatedContext(null);

        // A
        await _events.TokenValidated(tokenValidatedContext);

        // A
        _userRepositoryMock.Verify(u => u.GetByPredicateAsync(
            It.IsAny<Expression<Func<User, bool>>>(), 
            It.IsAny<CancellationToken>()), 
            Times.Never());
        
        _userRepositoryMock.Verify(u => u.GetAuthenticationTokenAsync(
            It.IsAny<User>()), 
            Times.Never());
    }

    [Fact]
    public async Task TokenValidated_UserDoesNotExist_CallsFail()
    {
        // A
        var token = new JwtSecurityToken(_validTokenValue);
        var tokenValidatedContext = TokenValidatedContextBuilder.BuildTokenValidatedContext(token);
        var tokenValidatedContextMock = new Mock<TokenValidatedContext>();

        _userRepositoryMock.Setup(u => u.GetByPredicateAsync(
            It.IsAny<Expression<Func<User, bool>>>(), 
            It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult<User?>(null));

        // A
        await _events.TokenValidated(tokenValidatedContext);

        // A
        _userRepositoryMock.Verify(u => u.GetByPredicateAsync(
            It.IsAny<Expression<Func<User, bool>>>(),
            It.IsAny<CancellationToken>()),
            Times.Once());

        _userRepositoryMock.Verify(u => u.GetAuthenticationTokenAsync(
            It.IsAny<User>()),
            Times.Never());

        Assert.False(tokenValidatedContext.Result.Succeeded);
    }

    [Fact]
    public async Task TokenValidated_TokensDoesntMatch_CallsFail()
    {
        // A
        var token = new JwtSecurityToken(_validTokenValue);
        var tokenValidatedContext = TokenValidatedContextBuilder.BuildTokenValidatedContext(token);

        var user = UserBuilder.NewObject().DomainBuild();
        _userRepositoryMock.Setup(u => u.GetByPredicateAsync(
            It.IsAny<Expression<Func<User, bool>>>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        const string alternativeTokenValue = "another";
        _userRepositoryMock.Setup(u => u.GetAuthenticationTokenAsync(
            It.IsAny<User>()))
            .ReturnsAsync(alternativeTokenValue);

        // A
        await _events.TokenValidated(tokenValidatedContext);

        // A
        Assert.False(tokenValidatedContext.Result.Succeeded);
    }
}
