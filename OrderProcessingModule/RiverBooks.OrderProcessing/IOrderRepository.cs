namespace RiverBooks.OrderProcessing;

internal interface IOrderRepository
{
    Task AddAsync(Order order);
    Task<List<Order>> ListAsync();
    Task SaveChangesAsync();
}
