
namespace RiverBooks.Books;

/*
* Remember, the application service, this book service, supports the application, which in this case is our Web Api
* application. As such, it's going to return Dtos in a form that the application can either use directly or or with
* minimal additional mapping.
*/
internal sealed class BookService : IBookService
{
    private readonly IBookRepository _bookRepository;

    public BookService(IBookRepository bookRepository)
    {
        _bookRepository = bookRepository;
    }

    public async Task CreateBookAsync(BookDto newBook)
    {
        var book = new Book(newBook.Id, newBook.Title, newBook.Author, newBook.Price);

        await _bookRepository.AddAsync(book);
        await _bookRepository.SaveChangesAsync();
    }

    public async Task DeleteBookAsync(Guid id)
    {
        var bookToDelete = await _bookRepository.GetByIdAsync(id);

        if (bookToDelete is not null)
        {
            await _bookRepository.DeleteAsync(bookToDelete);
            await _bookRepository.SaveChangesAsync();
        }
    }

    public async Task<BookDto> GetBookByIdAsync(Guid id)
    {
        var book = await _bookRepository.GetByIdAsync(id);

        // TODO: handle not found case

        return new BookDto(book!.Id, book.Title, book.Author, book.Price);
    }

    public async Task<List<BookDto>> ListBooksAsync()
    {
        var books = (await _bookRepository.ListAsync())
        .Select(x => new BookDto(x.Id, x.Title, x.Author, x.Price))
        .ToList();

        return books;
    }

    public async Task UpdateBookPriceAsync(Guid bookId, decimal newPrice)
    {
        /*
        * we're going to want to validate that price somewhere. We could do it here. We might also do it at 
        * the Api level. we could do it both. In any case, we're going to have to look up the book and then call its
        * method to update the price.
        */
        //validate the price

        var book = await _bookRepository.GetByIdAsync(bookId);

        // handle not found case

        book!.UpdatePrice(newPrice);
        await _bookRepository.SaveChangesAsync();        
    }
}
