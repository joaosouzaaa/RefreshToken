﻿using FluentValidation;
using RefreshTokenAuthentication.API.DataTransferObjects.User;
using RefreshTokenAuthentication.API.Entities;
using RefreshTokenAuthentication.API.Interfaces.Mappers;
using RefreshTokenAuthentication.API.Interfaces.Repositories;
using RefreshTokenAuthentication.API.Interfaces.Services;
using RefreshTokenAuthentication.API.Interfaces.Settings;

namespace RefreshTokenAuthentication.API.Services;

public sealed class UserService(
    IUserRepository userRepository,
    IUserMapper userMapper,
    IJwtService jwtService,
    IRefreshTokenService refreshTokenService,
    IValidator<User> userValidator,
    INotificationHandler notificationHandler)
    : IUserService
{
    public async Task CreateAsync(CreateUserRequest createUser, CancellationToken cancellationToken)
    {
        if (await userRepository.UserNameExistsAsync(createUser.Email, cancellationToken))
        {
            notificationHandler.AddNotification("Exists", "User Name already exists");

            return;
        }

        var user = userMapper.CreateRequestToDomain(createUser);

        if (!await IsValidAsync(user, cancellationToken))
        {
            return;
        }

        await userRepository.CreateAsync(user);
    }

    public async Task<GetUserByIdResponse?> GetByIdAsync(string id, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByPredicateAsync(u => u.Id == id, cancellationToken);

        if (user is null)
        {
            return null;
        }

        return userMapper.DomainToGetByIdResponse(user);
    }

    public async Task<BearerResponse?> LoginAsync(LoginRequest loginRequest, CancellationToken cancellationToken)
    {
        var loginArgument = userMapper.LoginRequestToDomain(loginRequest);

        if (!await userRepository.LoginAsync(loginArgument))
        {
            notificationHandler.AddNotification("Login Failed", "Invalid Credentials.");

            return null;
        }

        var user = await userRepository.GetByPredicateAsync(u => u.UserName == loginArgument.Email, cancellationToken);

        var token = jwtService.GenerateToken(user!);

        await userRepository.SetAuthenticationTokenAsync(user!, token);

        var refreshToken = await refreshTokenService.AddOrReplaceRefreshTokenAsync(user!.Id, cancellationToken);

        return new(token,
                   refreshToken.Value,
                   refreshToken.ExpiryDate);
    }

    public async Task SignOutAsync(string userId, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByPredicateAsync(u => u.Id == userId, cancellationToken);

        if (user is null)
        {
            notificationHandler.AddNotification("Not Found", "User not found");

            return;
        }

        await userRepository.RemoveAuthenticationTokenAsync(user);
    }

    private async Task<bool> IsValidAsync(User user, CancellationToken cancellationToken)
    {
        var validationResult = await userValidator.ValidateAsync(user, cancellationToken);

        if (validationResult.IsValid)
        {
            return true;
        }

        foreach (var error in validationResult.Errors)
        {
            notificationHandler.AddNotification(error.PropertyName, error.ErrorMessage);
        }

        return false;
    }
}
