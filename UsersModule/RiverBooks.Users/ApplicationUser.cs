using Ardalis.GuardClauses;
using Microsoft.AspNetCore.Identity;

namespace RiverBooks.Users;

public class ApplicationUser : IdentityUser
{
    public string FullName { get; set; } = string.Empty;

    /*
    * Now, whenever you're practicing domain driven design, you want to use encapsulation around you collections. So instead of
    * just exposing a list of cart item right on our user class, instead we're going to wrap that with a read-only collection
    * and reference a private read-only list of cart items, which nobody can set except us, and which we're going to ensure
    * is always instantiated. This will avoid null reference exceptions, and it also makes it so that we're not exposing all
    * of the operations that list of T supports, like clear and remove. We may not want to support that, or if we do want
    * to support it, we probably want to support it directly through our class and not just off of that list property. Case
    * in point, if we just exposed a list, then anybody could call .add on it, and we would have no control over that logic,
    * whereas we definitely want to control the logic of how you add an item to the cart as you're about to see.
    */
    private readonly List<CartItem> _cartItems = new();
    public IReadOnlyCollection<CartItem> CartItems => _cartItems.AsReadOnly();

    public void AddItemToCart(CartItem item)
    {
        Guard.Against.Null(item);

        var existingBook = _cartItems.SingleOrDefault(c => c.BookId == item.BookId);
        if (existingBook is not null)
        {
            existingBook.UpdateQuantity(existingBook.Quantity + item.Quantity);
            //TODO: what to do if other details of the item have been updated?
            return;
        }
        _cartItems.Add(item);
    }
}

public class CartItem
{
    public CartItem(Guid bookId, string description, int quantity, decimal unitPrice)
    {
        BookId = Guard.Against.Default(bookId);
        Description = Guard.Against.NullOrEmpty(description);
        Quantity = Guard.Against.Negative(quantity);
        UnitPrice = Guard.Against.Negative(unitPrice);
    }

    // EF
    public CartItem()
    {
    }


    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid BookId { get; private set; }
    public string Description { get; private set; } = string.Empty;
    public int Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }

    internal void UpdateQuantity(int quantity)
    {
        Quantity = Guard.Against.Negative(quantity);
    }
}
