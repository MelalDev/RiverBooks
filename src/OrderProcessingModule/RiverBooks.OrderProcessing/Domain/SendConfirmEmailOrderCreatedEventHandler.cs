using MediatR;
using RiverBooks.EmailSending.Contracts;
using RiverBooks.Users.Contracts;

namespace RiverBooks.OrderProcessing.Domain;

/*
* Now since this is a domain event, even though it's going to be interacting with other modules, it doesn't necessarily
* need to go in the integrations folder, it should go inside the domain folder.
*/
/*
* So now you've seen two different ways that we could set up sending emails. One of them is to directly try and send an email
* when we feel like we want to do that in a certain use case, add the other is to reactively send an email in response to
* a domain event by putting that logic into a domain event handler. Both of them have their place (UserCreate, CheckoutCart),
* and by using these two alternative ways of doing that work, it gives us more flexibility in how we handle that logic.
* In either case, it helps to ensure that we have some options and we don't end up making really long methods that have all
* the logic for sending emails or any other activity that needs to happen in that one method. By being able to just raise
* an event and have a handler be responsible for dealing with some aspect of follow-on logic that happens after some state
* change, it cleans up the code quite a bit.
*/
internal class SendConfirmEmailOrderCreatedEventHandler : INotificationHandler<OrderCreatedEvent>
{
    private readonly IMediator _mediator;

    public SendConfirmEmailOrderCreatedEventHandler(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task Handle(OrderCreatedEvent notification, CancellationToken ct)
    {
        var userByIdQuery = new UserDetailsByIdQuery(notification.Order.UserId);

        var result = await _mediator.Send(userByIdQuery);

        if (!result.IsSuccess)
        {
            // TODO: Log the error
            return;
        }
        string userEmail = result.Value.EmailAddress;

        var command = new SendEmailCommand
        {
            To = userEmail,
            From = "noreply@test.com",
            Subject = "Your RiverBooks Purchase",
            Body = $"You bought {notification.Order.OrderItems.Count} items."
        };

        Guid emailId = await _mediator.Send(command);

        // TODO: Do sth with emailId
    }
}
