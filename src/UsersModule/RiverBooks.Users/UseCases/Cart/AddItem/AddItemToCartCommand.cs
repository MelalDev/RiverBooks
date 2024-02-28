using Ardalis.Result;
using MediatR;

namespace RiverBooks.Users.UseCases.Cart.AddItem;

/*
* the `AddItemToCartCommand` command is actually part of sth else. That's not part of endpoint. That's part of what is typically
* called the use cases of this project. And this is where you can use command, query responsibility segregation (CQRS). So 
* inside of these use cases, some folks like to have separeate folder for commands and queries. I'm going to just leave it
* so that it's obvious what they are based on what their suffix is. And so in this case, this command is going to go in
* here (UseCases folder). And now, that command is living inside of the UseCases folder. And that's where we're going to
* put its handler also.
*/
public record AddItemToCartCommand(Guid BookId, int Quantity, string EmailAddress) : IRequest<Result>;
