using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace RiverBooks.Books.Data;

internal class BookConfiguration : IEntityTypeConfiguration<Book>
{
    internal static readonly Guid BookId1 = Guid.Parse("0cf1a38e-3a12-4ee3-9d61-427f429969a1");
    internal static readonly Guid BookId2 = Guid.Parse("28f9de3e-7584-4355-8b4e-cf301c159d30");
    internal static readonly Guid BookId3 = Guid.Parse("25b699ec-cd96-432a-b676-407f3ee90ebe");
    public void Configure(EntityTypeBuilder<Book> builder)
    {
        builder.Property(p => p.Title)
            /*
            * I like to use constants, so they don't have a lot of magic numbers in here for different lengths of things.
            * And since names are pretty common, whether it's people's names or names of books, names of authors, things
            * like that, I find that having a default length for all my names really helps. 
            */
            .HasMaxLength(DataSchemaConstants.DEFAULT_NAME_LENGTH)
            .IsRequired();

        builder.Property(p => p.Author)
            .HasMaxLength(DataSchemaConstants.DEFAULT_NAME_LENGTH)
            .IsRequired();

        /*
        * it's nice would be for us to have some sample data to work with. If you're building a production application, maybe
        * yuo have some seed data for lookup tables. In this case, since this is a demo for a course, I just want to fill it
        * up with some data that we can use as we start to work with the Api. So for that, I'm going to take my token book
        * data and I'm going to put it into a little helper method here and then say that this particular table has that data
        */
        builder.HasData(GetSampleBookData());
    }

    private IEnumerable<Book> GetSampleBookData()
    {
        var tolkien = "J.R.R Tolkien";
        yield return new Book(BookId1, "The Fellowship of the Ring", tolkien, 10.99m);
        yield return new Book(BookId2, "The Two Towers", tolkien, 11.99m);
        yield return new Book(BookId3, "The Return of the King", tolkien, 12.99m);

    }
}