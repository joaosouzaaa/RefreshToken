using RefreshTokenAuthentication.API.Arguments;
using RefreshTokenAuthentication.API.DataTransferObjects.User;
using RefreshTokenAuthentication.API.Entities;

namespace UnitTests.TestBuilders;

internal sealed class UserBuilder
{
    private readonly string _id = Guid.NewGuid().ToString();
    private string _userName = "test@email.com";
    private string _password = "te123st";

    public static UserBuilder NewObject() =>
        new();

    public User DomainBuild() =>
        new()
        {
            Id = _id,
            Email = _userName,
            UserName = _userName,
            PasswordHash = _password
        };

    public CreateUserRequest CreateRequestBuild() =>
        new(_userName,
            _password);

    public GetUserByIdResponse GetByIdResponseBuild() =>
        new(_id,
            _userName);

    public LoginRequest LoginRequestBuild() =>
        new(_userName,
            _password);

    public LoginArgument LoginArgumentBuild() =>
        new(_userName,
            _password);

    public UserBuilder WithUserName(string userName)
    {
        _userName = userName;

        return this;
    }

    public UserBuilder WithPassword(string password)
    {
        _password = password;

        return this;
    }
}
