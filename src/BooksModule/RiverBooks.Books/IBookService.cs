namespace RiverBooks.Books;

// make this internal internal so that you couldn't wire those up in Program.cs in RiverBooks.Web even if you wanted to
internal interface IBookService
{
    Task<List<BookDto>> ListBooksAsync();
    Task<BookDto> GetBookByIdAsync(Guid id);
    Task CreateBookAsync(BookDto newBook);
    Task DeleteBookAsync(Guid id);
    Task UpdateBookPriceAsync(Guid bookId, decimal newPrice);
}
