using RefreshTokenAuthentication.API.Validators;
using UnitTests.TestBuilders;

namespace UnitTests.ValidatorsTests;

public sealed class UserValidatorTests
{
    private readonly UserValidator _userValidator;

    public UserValidatorTests()
    {
        _userValidator = new UserValidator();
    }

    [Fact]
    public async Task ValidateAsync_SuccessfulScenario_ReturnsTrue()
    {
        // A
        var user = UserBuilder.NewObject().DomainBuild();

        // A
        var validationResult = await _userValidator.ValidateAsync(user);

        // A
        Assert.True(validationResult.IsValid);
    }

    [Theory]
    [MemberData(nameof(InvalidUserNameParameters))]
    public async Task ValidateAsync_InvalidUserName_ReturnsFalse(string userName)
    {
        // A
        var userWithInvalidUserName = UserBuilder.NewObject().WithUserName(userName).DomainBuild();

        // A
        var validationResult = await _userValidator.ValidateAsync(userWithInvalidUserName);

        // A
        Assert.False(validationResult.IsValid);
    }

    public static TheoryData<string> InvalidUserNameParameters() =>
        new()
        {
            string.Empty,
            "test",
            "rasd",
            "a",
            "invalid@",
            "inva.com1",
            $"suc@{new string('a', 101)}.com"
        };

    [Theory]
    [MemberData(nameof(InvalidPasswordParameters))]
    public async Task ValidateAsync_InvalidPassword_ReturnsFalse(string password)
    {
        // A
        var userWithInvalidPassword = UserBuilder.NewObject().WithPassword(password).DomainBuild();

        // A
        var validationResult = await _userValidator.ValidateAsync(userWithInvalidPassword);

        // A
        Assert.False(validationResult.IsValid);
    }

    public static TheoryData<string> InvalidPasswordParameters() =>
        new()
        {
            string.Empty,
            "test",
            new string('a', 101)
        };
}
