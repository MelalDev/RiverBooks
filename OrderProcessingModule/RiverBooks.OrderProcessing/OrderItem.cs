using Ardalis.GuardClauses;

namespace RiverBooks.OrderProcessing;

internal class OrderItem
{
    private OrderItem()
    {
    }

    public OrderItem(Guid bookId, int quantity, decimal unitPrice, string desciption)
    {
        BookId = Guard.Against.Default(bookId);
        Quantity = Guard.Against.Negative(quantity);
        UnitPrice = Guard.Against.Negative(unitPrice);
        Description = Guard.Against.NullOrEmpty(desciption);
    }

    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid BookId { get; private set; }
    public int Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }
    public string Description { get; private set; } = string.Empty;
}
