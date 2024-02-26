using Ardalis.Result;
using MediatR;
using RiverBooks.Books.Contracts;

namespace RiverBooks.Books.Integrations;

/*
* I'm putting it into a folder called Integrations, so it's clear that this is for integrating with other modules. You may have
* other handlers for handling domain events or other things inside of your domain, inside of this module. We want to do
* up instead of this exists a few requests that are coming from other modules.
*/
internal class BookDetailsQueryHandler : IRequestHandler<BookDetailsQuery, Result<BookDetailsResponse>>
{
    private readonly IBookService _bookService;

    public BookDetailsQueryHandler(IBookService bookService)
    {
        _bookService = bookService;
    }

    public async Task<Result<BookDetailsResponse>> Handle(BookDetailsQuery request, CancellationToken cancellationToken)
    {
        var book = await _bookService.GetBookByIdAsync(request.BookId);

        if (book is null)
        {
            return Result.NotFound();
        }

        var response = new BookDetailsResponse(book.Id, book.Title, book.Author, book.Price);

        return response;
    }
}