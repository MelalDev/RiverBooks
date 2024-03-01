using MediatR;
using RiverBooks.EmailSending.Contracts;
using RiverBooks.SharedKernel;

namespace RiverBooks.OrderProcessing.Domain;

internal class OrderCreatedEvent : DomainEventBase
{
    public OrderCreatedEvent(Order order)
    {
        Order = order;
    }

    public Order Order { get; }
}

/*
* Now since this is a domain event, even though it's going to be interacting with other modules, it doesn't necessarily
* need to go in the integrations folder, it should go inside the domain folder.
*/
internal class SendConfirmEmailOrderCreatedEventHandler : INotificationHandler<OrderCreatedEvent>
{
    private readonly IMediator _mediator;

    public SendConfirmEmailOrderCreatedEventHandler(IMediator mediator)
    {
        _mediator = mediator;
    }

    public Task Handle(OrderCreatedEvent notification, CancellationToken ct)
    {
        var userByIdQuery = new UserDetailsByIdQuery(notification.Order.UserId);

        var command = new SendEmailCommand
        {
            To = notification.Order,
            From = "noreply@test.com",
            Subject = "Your RiverBooks Purchase",
            Body = $"You bought {notification.Order.OrderItems.Count} items."
        };
        Guid emailId = await _mediator.Send(command);

    }
}