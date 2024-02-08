namespace RiverBooks.Books;

// make this internal internal so that you couldn't wire those up in Program.cs in RiverBooks.Web even if you wanted to
internal interface IBookService
{
    List<BookDto> ListBooks();
}
