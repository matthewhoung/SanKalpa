using SanKalpa.Domain.Abstrations;
using SanKalpa.Domain.Users.Events;
using SanKalpa.Domain.Users.ValueObjects;

namespace SanKalpa.Domain.Users;

public sealed class User : Entity
{
    public UserName UserName { get; private set; }
    public EmailAddress EmailAddress { get; private set; }
    public Password UserPassword { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    private User()
    {
    }

    private User(
        Guid id,
        UserName userName,
        EmailAddress emailAddress,
        Password userPassword) 
        : base(id)
    {
        UserName = userName;
        EmailAddress = emailAddress;
        UserPassword = userPassword;
        CreatedAt = DateTime.Now;
        UpdatedAt = DateTime.Now;
    }

    public static User Create(
        UserName userName,
        EmailAddress emailAddress,
        Password userPassword)
    {
        var user = new User(
            Guid.NewGuid(),
            userName,
            emailAddress,
            userPassword);

        user.RaiseDomainEvent(new UserCreatedDomainEvent(user.Id));

        return user;
    }
}
