﻿namespace RefreshToken.API.Entities;

public sealed class ApplicationRefreshToken
{
    public Guid Id { get; set; }
    public required string Value { get; set; }
    public required DateTime ExpiryDate { get; set; }

    public required string UserId { get; set; }
    public User User { get; set; } = null!;
}
