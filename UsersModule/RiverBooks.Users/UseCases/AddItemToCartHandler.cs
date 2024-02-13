using Ardalis.Result;
using MediatR;

namespace RiverBooks.Users.UseCases;

public class AddItemToCartHandler : IRequestHandler<AddItemToCartCommand, Result>
{
    private readonly IApplicationUserRepository _userRepository;

    public AddItemToCartHandler(IApplicationUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<Result> Handle(AddItemToCartCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetUserWithCardByEmail(request.EmailAddress);

        if (user is null)
        {
            return Result.Unauthorized();
        }

        // TODO: get description and price from books module
        var newCartItem = new CartItem(request.BookId, "description", request.Quantity, 1.00m);

        user.AddItemToCart(newCartItem);

        await _userRepository.SaveChangesAsync();

        return Result.Success();
    }
}
