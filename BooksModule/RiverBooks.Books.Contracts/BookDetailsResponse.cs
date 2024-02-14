/*
* That (Contracts folder) is the thing that's going to be used to communicate with other modules. And you can call that
* sth like public Api, public interface, contracts. I'm going to go with Contracts in this case because this is where
* the messages are going to live that are public, that are discoverable by other modules. And we're going to see
* in a little bit, they may not stay in this project.
*/
namespace RiverBooks.Books.Contracts;

/*
* we don't want to call it just book or book entity or sth that's close to the domain model inside the books module.
* But a good example of sth that you can call things like this is book details. It's not going to be confused with
* your BookDto that you're using your Api. And it's just another synonym for the types of information you might exchange
* between modules in you modular monolith.
*/
public record BookDetailsResponse(Guid BookId, string Title, string Author, decimal Price);
