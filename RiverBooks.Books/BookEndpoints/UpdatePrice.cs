using FastEndpoints;

namespace RiverBooks.Books.BookEndpoints;

internal class UpdatePrice(IBookService bookService) : Endpoint<UpdateBookPriceRequest, BookDto>
{
    private readonly IBookService _bookService = bookService;

    public override void Configure()
    {
        /*
        * When it comes to update, when you're building rest-based Api endpoints, normally you would do a put and
        * replace the entire entity or resource with all of its new values. In this case, we're really doing more
        * of an RPC style where we're just trying to call one method on the entity to update its price, and we
        * don't necessarily want to have to pass back all the information about the resource in order to do it.
        * Another way to do it is to do a post, but instead of posting to just do the RPC call to just call that
        * method, we're going to do a post to a collection of prices, and so we're going to update the price
        * to be the most recent price, but imagine that there's this priceHistory endpoint that we could get to
        * that was hanging off book. At the moment, we haven't implemented that, but there's nothing to say
        * that we couldn't do so in the future, so right now if you do a get on bookPriceHistory, you won't
        * get anything, but we can post there and use that as a way to allow us to send a simple message with
        * just the price for that book and still be following the rules of typical rest API design.
        */
        Post("/books/{Id}/pricehistory");
        AllowAnonymous();
    }

    public override async Task HandleAsync(UpdateBookPriceRequest req, CancellationToken ct)
    {
        await _bookService.UpdateBookPriceAsync(req.Id, req.NewPrice);

        var updatedBook = await _bookService.GetBookByIdAsync(req.Id);

        await SendAsync(updatedBook);
    }
}
