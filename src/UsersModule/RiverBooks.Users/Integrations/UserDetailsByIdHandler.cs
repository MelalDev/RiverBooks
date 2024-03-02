
using Ardalis.Result;
using MediatR;
using RiverBooks.Users.Contracts;
using RiverBooks.Users.UseCases.User.GetById;

namespace RiverBooks.Users.Integrations;

/*
* Now these integrations, you might be tempted to just have them work directly with your domain objects. I prefer to have
* these interact with use case just like your endpoints would. Remember, these integrations and your endpoints represent
* the user interface for a module, and so you want to have your use cases be the only thing that those user interface
* elements interact with. So we're going to translate this UserDetailsByIdQuery, that is an integration contract, and
* turn that into a use case type that we can use, and essentially delegate to the use case to do the actual lookup
* for the user.
*/
internal class UserDetailsByIdHandler : IRequestHandler<UserDetailsByIdQuery, Result<UserDetails>>
{
    private readonly IMediator _mediator;

    public UserDetailsByIdHandler(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task<Result<UserDetails>> Handle(UserDetailsByIdQuery request, CancellationToken cancellationToken)
    {
        var query = new GetUserByIdQuery(request.UserId);

        var result = await _mediator.Send(query);

        return result.Map(u => new UserDetails(u.UserId, u.EmailAddress));
    }
}
