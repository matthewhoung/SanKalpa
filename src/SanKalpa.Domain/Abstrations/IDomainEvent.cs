using MediatR;

namespace SanKalpa.Domain.Abstrations;

public interface IDomainEvent : INotification
{
    Guid Id { get; }
    DateTime OccurredOn { get; }

    public abstract record DomainEvent : IDomainEvent
    {
        public Guid Id { get; } = Guid.NewGuid();
        public DateTime OccurredOn { get; } = DateTime.Now;
    }
}
