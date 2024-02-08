namespace RiverBooks.Books;

/*
* For the response, I like to have strongly typed responses for each one of my endpoints so that I can have complete
* control over what goes into that. In the response, that will have the payload of the set of books. Our endpoints
* is going to change slightly from how it was. We're going to have a class that wraps our list of BookDto.
*/
public class ListBooksResponse
{
    public List<BookDto> Books { get; set; } = [];
}