using Ardalis.Result;
using MediatR;
using RiverBooks.Users.CartEndpoints;

namespace RiverBooks.Users.UseCases.Cart.ListItems;

internal class ListCartItemsQueryHandler : IRequestHandler<ListCartItemsQuery, Result<List<CartItemDto>>>
{
    /*
    * Another good reason to favor using an abstraction here and not a full Db context is because of the interface
    * segregation principle. This is ensuring that we're not depending on addtional methods in that type that we
    * aren't using. Inside of that user repository, there's only two methods. And for this one, we're only using
    * one of them. In the other case where we add an item, we're using both. But if we were passing around a Db context,
    * it has a huge interface and we're not using hardly any of it. So it's a way bigger type to pass around than
    * we would like. The reason why that principle exists is because it makes it much more difficult for you to verify
    * that you're testing every case of sth or that a change to sth desn't break other things when you pass around
    * a type that has a ton of methods and you're not actually using all of them.
    */
    private readonly IApplicationUserRepository _userRepository;

    public ListCartItemsQueryHandler(IApplicationUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<Result<List<CartItemDto>>> Handle(ListCartItemsQuery request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetUserWithCardByEmailAsync(request.EmailAddress);

        if (user is null)
        {
            return Result.Unauthorized();
        }

        return user.CartItems
            .Select(x => new CartItemDto(x.Id, x.BookId, x.Description, x.Quantity, x.UnitPrice))
            .ToList();
    }
}
