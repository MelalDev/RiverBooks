using Ardalis.GuardClauses;

namespace RiverBooks.Books;

/*
* This book entity is part of domain model, which we want to ensure this always in a valid state.
* Your domain model entities should define cetain invariants that are always true and can't be broken, so at a minimum
* for the book entity, we're going to say that the Id should never be empty, the title and author should not be null or
* empty, and the price should never be negative. Now we could implement some of these rules by using custom types or
* value objects instead of the GUID, string, and decimal types that we're using here, but for now, we're going to keep
* these primitive types. We're going to enfore these rules using guard clauses, and I'm going to add our Dallas 
* .guard clauses nuget to help with this.
*/
internal sealed class Book
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string Title { get; private set; } = string.Empty;
    public string Author { get; private set; } = string.Empty;
    public decimal Price { get; private set; }

    /*
    * Now some of you may be wondering, why are we are using guard clauses that throw exceptions instead of validation?
    * The reason has to do with how your domain model should be designed. Your domain should have certain invariants that
    * its design ensures are always true.
    * Think about the Datetime in .Net. Have you ever had to check an instance of date time to see if the month was valid?
    * No, because you can't actually create a date time with a month of 27. Imagine how much harder it would be to work
    * with the date time type if you always needed to validate it and ensure it was good to go before you could use it.
    * This is the same approach you want to take with your domain model. We will have validation, but that is an application
    * concern and meant to help users provide proper input, but not in the domain model. The domain model should always
    * be valid. Its design should enforce that it's always valid.
    * And so we're going to guard against invalid operations by treating those as bugs, failures in our application layer,
    * whenever we see that some invalid data is enterting our domain layer.
    */

    internal Book(Guid id, string title, string author, decimal price)
    {
        Id = Guard.Against.Default(id); //should not be empty
        Title = Guard.Against.NullOrEmpty(title); //should not be null or empty
        Author = Guard.Against.NullOrEmpty(author); //should not be null or empty
        Price = Guard.Against.Negative(price); //should not be negative number
    }

    internal void UpdatePrice(decimal newPrice)
    {
        /*
        * Now, there is a little bit of dupplication here, but I prefer this dupplication to having them be tightly coupled
        * because sometimes when you create sth, it has different invariants than when you go to update an individual 
        * property, and in any case, they're both going to be in the same file. It should be pretty easy to keep in sync.
        * If you really want to get rid of that dupplication, you could say guard against invalid price, and then have
        * your own custom guard for that, and then use that both places to make sure that the same rules were being applied.
        */
        Price = Guard.Against.Negative(newPrice); //should not be negative number
    }
}
