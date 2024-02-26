using System.Security.Claims;
using Ardalis.Result;
using FastEndpoints;
using MediatR;
using RiverBooks.Users.UseCases.Cart.AddItem;

namespace RiverBooks.Users.CartEndpoints;

internal class AddItem : Endpoint<AddCartItemRequest>
{
    private readonly IMediator _mediator;

    public AddItem(IMediator mediator)
    {
        _mediator = mediator;
    }
    /*
    * we're going to assume that this endpoint is going to be requiring authentication. So we'll get the user's information
    * from their token. So really all we need to know is the book ID and how many of that book they want. Now you could argure
    * that we might also ask for the title, the author, the price. Any of that information could also get sent to us over
    * the API, but remember, if this is coming from a single page application or a mobile app or something like that, we don't
    * necessarily want to trust that application to give us information like the price of the book that they're going to 
    * be purchasing, because anybody could just use Postman or some other tool to submit and add items to the cart. And if
    * they can send whatever price they want, then it's going to be really expensive for us when people start buying books
    * for a penny. So we want to make sure that the price data through some secure mechanism and not from the client who
    * is submitting this request.
    */
    public override void Configure()
    {
        Post("/cart");
        /*
        * Okay, so we're going to set this up so that they're going to post the cart endpoint. And instead of saying 
        * AllowAnonymous, we're going to specify that we need a claim called email address in order for this things to work.
        */
        Claims("EmailAddress");
    }

    public override async Task HandleAsync(AddCartItemRequest req, CancellationToken ct)
    {
        //search for the claim we're looking for
        var emailAddress = User.FindFirstValue("EmailAddress");

        /*
        * all right, now the question at this point is what needs to go inside of that command? We need to provide the handler
        * with all the information it needs, which is going to be the BookId, the quantity, and the email address that the
        * user is using. So the only thing the endpoint is really doing is handling the authentication and pulling that email
        * address out of the claims. And then everything else can be done by the handler.
        */
        var command = new AddItemToCartCommand(req.BookId, req.Quantity, emailAddress!);

        var result = await _mediator!.Send(command, ct);

        if (result.Status == ResultStatus.Unauthorized)
        {
            await SendUnauthorizedAsync(ct);
        }
        else
        {
            await SendOkAsync();
        }
    }
}