using MediatR;

namespace RiverBooks.SharedKernel;

public abstract class DomainEventBase : INotification
{
    public DateTime DateOccured { get; protected set; } = DateTime.UtcNow;
}
