namespace RiverBooks.Books;

public sealed record BookDto(Guid Id, string Title, string Author, decimal Price);