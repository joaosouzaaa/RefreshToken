﻿namespace RefreshToken.API.DataTransferObjects.User;

public sealed record GetUserByIdResponse(
    string Id,
    string Email);
