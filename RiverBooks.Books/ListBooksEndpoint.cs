using FastEndpoints;

namespace RiverBooks.Books;

//https://fast-endpoints.com
/*
* the way FastEndpoints works is each class inherits from an endpoint class. It's using the Reaper design pattern.
* It's a RequestEndpointResponse, or REPR, Reaper. This is the endpoint, it could also have a request or a response
* For a list endpoint, we don't need to pass it a request. At this point, maybe if we add paging or sth later,
* we'll pass in a request for what page you want. For now, we're just saying list all the books.
*/
internal class ListBooksEndpoint(IBookService bookService) : EndpointWithoutRequest<ListBooksResponse>
{
    /*
    * For our endpoint, there are two methods that we need to implement for FastEndpoints. The first one is the one
    * that configures the route and other attributes of the endpoint, and then the second one is a handler method 
    * that is actually the code that gets run when the endpoint is hit.
    */
    private readonly IBookService _bookService = bookService;

    public override void Configure()
    {
        Get("/books");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken cancellationToken = default)
    {
        var books = await _bookService.ListBooksAsync();

        /*
        * So the complete list books endpoint should look like this. We're going to have a task handleAsync that is going to
        * take in a cancellation token. Inside of there, we're just going to grab a list of books, and we're going to call
        * await SendAsync to send that wrapped up in a response.
        */
        await SendAsync(new ListBooksResponse
        {
            Books = books
        });
    }
}
