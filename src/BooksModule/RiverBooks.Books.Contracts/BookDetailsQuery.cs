/*
* That (Contracts folder) is the thing that's going to be used to communicate with other modules. And you can call that
* sth like public Api, public interface, contracts. I'm going to go with Contracts in this case because this is where
* the messages are going to live that are public, that are discoverable by other modules. And we're going to see
* in a little bit, they may not stay in this project.
*/
using Ardalis.Result;
using MediatR;

namespace RiverBooks.Books.Contracts;

public record BookDetailsQuery(Guid BookId) : IRequest<Result<BookDetailsResponse>>;