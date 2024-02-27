namespace RiverBooks.OrderProcessing.Domain;

internal class Order
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid UserId { get; private set; }
    public Address ShippingAddress { get; private set; } = default!;
    public Address BillingAddress { get; private set; } = default!;
    private readonly List<OrderItem> _orderItems = [];
    public IReadOnlyCollection<OrderItem> OrderItems => _orderItems.AsReadOnly();

    public DateTimeOffset DateCreated { get; private set; } = DateTimeOffset.Now;

    private void AddOrderItem(OrderItem item) => _orderItems.Add(item);

    /*
    * Now, later on, we're going to want to be able to create domain events whenever an order is created, and so rather than
    * just newing it up here (CreateOrderCommandHandler) and then trying to figure out how to create domain event from
    * within this integration, it's often useful to have a factory for creating your domain types. So we will create a factory
    * class that put inside the Order class, and it's responsible for actually instantiating the Order, and later on, adding
    * domain events
    */
    internal class Factory
    {
        /*
        * Now you may wonder why we're putting this inside of the order class. The reason is that this factory as a nested
        * class can actually access all the private setters and properties on Order.
        */
        public static Order Create(Guid userId,
            Address shippingAddress,
            Address billingAddress,
            IEnumerable<OrderItem> orderItems)
        {
            var order = new Order();
            order.UserId = userId;
            order.ShippingAddress = shippingAddress;
            order.BillingAddress = billingAddress;
            foreach (var item in orderItems)
            {
                order.AddOrderItem(item);
            }

            return order;
        }
    }
}
