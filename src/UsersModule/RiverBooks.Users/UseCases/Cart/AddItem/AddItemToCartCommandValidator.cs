using FluentValidation;

namespace RiverBooks.Users.UseCases.Cart.AddItem;

/*
 * Now it's worth noting that you can also add these types of validators to your fast endpoints and they will work just
 * fine there as well. So if you want to do that validation at the DTO level with the APIs, you can. I'm just showing
 * how you can do it yourself inside of your own use cases because invitably Microsoft is going to change how they
 * want to do web APIs and so you might switch from fast endpoints to some other library or to whatever the next thing
 * ASP.NET Core team comes up with, and you migh have to throw away or repurpose all you validation logic. Whereas
 * if you have it here, you own this pipeline and you have control over how it'se setup and when it runs and all of that.
 * So it's separate from those user interface concerns and it's not dependent on ASP.NET Core or fast endpoints.
 */
public class AddItemToCartCommandValidator : AbstractValidator<AddItemToCartCommand>
{
    public AddItemToCartCommandValidator()
    {
        RuleFor(x => x.EmailAddress)
            .NotEmpty()
            .WithMessage("EmailAddress is required.");

        RuleFor(x => x.Quantity)
            .GreaterThan(0)
            .WithMessage("Quantity must be at least 1.");

        RuleFor(x => x.BookId)
            .NotEmpty()
            .WithMessage("Not a valid BookId.");
    }
}
