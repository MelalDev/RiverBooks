using MediatR;
using Microsoft.Extensions.Logging;
using RiverBooks.OrderProcessing.Domain;
using RiverBooks.OrderProcessing.Interfaces;
using RiverBooks.Users.Contracts;

namespace RiverBooks.OrderProcessing.Integrations;


/*
* All right, so we're going to call this the `address cache updating new user address handler`. It's the best name I could
* come up with, and it's going to be a mediator notification handler for new user address added integration event, which
* we need to add a reference to contracts for, specifically users contracts
*/
internal class AddressCacheUpdatingNewUserAddressHandler : INotificationHandler<NewUserAddressAddedIntegrationEvent>
{
    private readonly IOrderAddressCache _addressCache;
    private readonly ILogger<AddressCacheUpdatingNewUserAddressHandler> _logger;

    public AddressCacheUpdatingNewUserAddressHandler(ILogger<AddressCacheUpdatingNewUserAddressHandler> logger, IOrderAddressCache addressCache)
    {
        _logger = logger;
        _addressCache = addressCache;
    }

    public async Task Handle(NewUserAddressAddedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        /*
        * convert integration event into an OrderAddress. So, we're getting an event that's defined in a different module
        * We need to translate that into the data that we have in out module. In this case, this order address is the
        * type that we're using with Redis. So it has the ID, as well as, all the details of address. 
        */
        var orderAddress = new OrderAddress(notification.Details.AddressId,
            new Address(notification.Details.Street1,
                notification.Details.Street2,
                notification.Details.City,
                notification.Details.State,
                notification.Details.PostalCode,
                notification.Details.Country
            ));

        await _addressCache.StoreAsync(orderAddress);

        _logger.LogInformation("Cache updated with new address {address}", orderAddress);
    }
}
