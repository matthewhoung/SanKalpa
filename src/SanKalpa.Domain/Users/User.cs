using SanKalpa.Domain.Abstrations;
using SanKalpa.Domain.Users.Events;
using SanKalpa.Domain.Users.ValueObjects;

namespace SanKalpa.Domain.Users;

public sealed class User : Entity
{
    public string IdentityId { get; private set; } = string.Empty;
    public FirstName FirstName { get; private set; }
    public LastName LastName { get; private set; }
    public EmailAddress EmailAddress { get; private set; }

    private User()
    {
    }

    private User(
        Guid id,
        FirstName firstName,
        LastName lastName,
        EmailAddress emailAddress)
        : base(id)
    {
        FirstName = firstName;
        LastName = lastName;
        EmailAddress = emailAddress;
    }

    public static User Create(
        FirstName firstName,
        LastName lastName,
        EmailAddress emailAddress)
    {
        var user = new User(
            Guid.NewGuid(),
            firstName,
            lastName,
            emailAddress);

        user.RaiseDomainEvent(new UserCreatedDomainEvent(user.Id));
        return user;
    }

    public void SetIdentityId(string identityId)
    {
        IdentityId = identityId;
    }

    public void UpdateNames(FirstName firstName, LastName lastName)
    {
        FirstName = firstName;
        LastName = lastName;
    }
}
