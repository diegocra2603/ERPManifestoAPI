using Domain.Primitives.Mediator;

namespace Domain.Primitives;

public record DomainEvents(Guid Id) : INotification;