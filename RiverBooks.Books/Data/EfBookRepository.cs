
using Microsoft.EntityFrameworkCore;

namespace RiverBooks.Books.Data;

/*
* Now we've decided we're going to have a separate saveChanges method on the repository, so we're not using
* a unit of work, and we're not automatically completing each of these operations as they are called, and so for some
* of these, even though we've defined them as async, they're just going to return a completed task.
*/
internal class EfBookRepository : IBookRepository
{
    private readonly BookDbContext _dbContext;

    public EfBookRepository(BookDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task AddAsync(Book book)
    {
        _dbContext.Add(book);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Book book)
    {
        _dbContext.Remove(book);
        return Task.CompletedTask;
    }

    public async Task<Book?> GetByIdAsync(Guid id)
    {
        return await _dbContext!.Books.FindAsync(id);
    }

    public async Task<List<Book>> ListAsync()
    {
        return await _dbContext.Books.ToListAsync();
    }

    public async Task SaveChangesAsync()
    {
        await _dbContext.SaveChangesAsync();
    }
}
