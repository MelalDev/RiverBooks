using Ardalis.Result;
using MediatR;
using RiverBooks.Books.Contracts;

namespace RiverBooks.Users.UseCases.Cart.AddItem;

public class AddItemToCartHandler : IRequestHandler<AddItemToCartCommand, Result>
{
    private readonly IApplicationUserRepository _userRepository;

    /*
    * Some of you may be wondering, is it okay to put mediator inside of a mediator handler so that when you issue one call,
    * it ends up triggering additional calls? Yes, it's totally fine. You can take that a little bit too far and end up
    * with a really complicated mess. But in this case, we're only doing one command that then is making one query. So
    * it's not complicated. It's not too terribly difficult for anyone to follow.
    */
    private readonly IMediator _mediator;

    public AddItemToCartHandler(IApplicationUserRepository userRepository, IMediator mediator)
    {
        _userRepository = userRepository;
        _mediator = mediator;
    }

    public async Task<Result> Handle(AddItemToCartCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetUserWithCardByEmailAsync(request.EmailAddress);

        if (user is null)
        {
            return Result.Unauthorized();
        }

        // TODO: get description and price from books module
        var query = new BookDetailsQuery(request.BookId);

        var result = await _mediator.Send(query, cancellationToken);

        if (result.Status == ResultStatus.NotFound) return Result.NotFound();

        var bookDetails = result.Value;
        string description = $"{bookDetails.Title} by {bookDetails.Author}";
        var newCartItem = new CartItem(request.BookId, description, request.Quantity, bookDetails.Price);

        user.AddItemToCart(newCartItem);

        await _userRepository.SaveChangesAsync();

        return Result.Success();
    }
}
