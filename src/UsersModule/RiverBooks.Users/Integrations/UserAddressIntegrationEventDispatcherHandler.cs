
using MediatR;
using Microsoft.Extensions.Logging;
using RiverBooks.Users.Contracts;

namespace RiverBooks.Users.Integrations;

internal class UserAddressIntegrationEventDispatcherHandler : INotificationHandler<AddressAddedEvent>
{
    private readonly IMediator _mediator;
    private readonly ILogger<UserAddressIntegrationEventDispatcherHandler> _logger;

    public UserAddressIntegrationEventDispatcherHandler(IMediator mediator, ILogger<UserAddressIntegrationEventDispatcherHandler> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /*
    * All right, in this method, what we're going to do is we're going to take in the information that we have about the
    * address, and then we're going to create a new user address details type, which is going to have additional information.
    * Frequently, your domain events don't need to have a lot of data in them. They can just have an ID, and if anything
    * inside the domain needs more information, it can look it up using a repository, or it might be already be in a Db context,
    * identity map. So, there's very little cost to finding extra details inside the domain. However, when you are sending
    * integration event, and it's going outside of domain, a lot of the time you're going to want to hydrate that event with
    * more details. You're going to make sure that it has all the information necessary for sth to react to that event, ideally.
    * You may not be able to get everything, you know, you don't necessarily want to have the entire object graph, but you
    * should have at least the basics, so it's not immediately call back to you and saying, well, give me the details on
    * this ID that you sent me.
    */
    public async Task Handle(AddressAddedEvent notification, CancellationToken ct)
    {
        Guid userId = Guid.Parse(notification.NewAddress.UserId);

        var addressDetail = new UserAddressDetails(userId,
            notification.NewAddress.Id,
            notification.NewAddress.StreetAddress.Street1,
            notification.NewAddress.StreetAddress.Street2,
            notification.NewAddress.StreetAddress.City,
            notification.NewAddress.StreetAddress.State,
            notification.NewAddress.StreetAddress.PostalCode,
            notification.NewAddress.StreetAddress.Country);

        await _mediator.Publish(new NewUserAddressAddedIntegrationEvent(addressDetail));

        _logger.LogInformation("[DE Handler]New address integration event sent for {user}: {address}",
            notification.NewAddress.UserId,
            notification.NewAddress.StreetAddress);
    }
}