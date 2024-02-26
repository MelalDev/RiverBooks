using System.Xml.XPath;
using Ardalis.Result;
using MediatR;

namespace RiverBooks.Users.Contracts;

public record NewUserAddressAddedIntegrationEvent(UserAddressDetails Details) : IntegrationEventBase;

public abstract record IntegrationEventBase : INotification
{
    public DateTimeOffset DateTimeOffset { get; set; } = DateTimeOffset.UtcNow;
}

public record UserAddressDetailsByIdQuery(Guid AddressId) : IRequest<Result<UserAddressDetails>>;