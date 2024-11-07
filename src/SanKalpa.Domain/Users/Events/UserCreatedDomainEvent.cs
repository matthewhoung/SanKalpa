using SanKalpa.Domain.Abstrations;

namespace SanKalpa.Domain.Users.Events;

public sealed record class UserCreatedDomainEvent(Guid UserId) : IDomainEvent;
