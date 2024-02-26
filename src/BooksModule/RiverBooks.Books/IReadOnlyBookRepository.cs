namespace RiverBooks.Books;

/*
* One more thing we can do, which I like to do, is separate out the read-only operations from those that are going to mutate
* the state of the system.
*/
internal interface IReadOnlyBookRepository
{
    Task<Book?> GetByIdAsync(Guid id);
    Task<List<Book>> ListAsync();
}
