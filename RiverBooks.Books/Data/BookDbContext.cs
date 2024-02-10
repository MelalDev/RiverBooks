using System.Reflection;
using Microsoft.EntityFrameworkCore;

namespace RiverBooks.Books.Data;

public class BookDbContext : DbContext
{
    internal DbSet<Book> Books { get; set; }

    public BookDbContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("Books");

        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        /*
        * This last one just make sure that all our prices are going to be stored in the database with a precision of
        * 18 comma 6, so it'll be consistent for all of our columns, which for now is just 1
        */
        configurationBuilder.Properties<decimal>().HavePrecision(18, 6);
    }
}
