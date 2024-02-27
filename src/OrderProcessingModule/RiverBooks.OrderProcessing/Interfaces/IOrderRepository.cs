using RiverBooks.OrderProcessing.Domain;

namespace RiverBooks.OrderProcessing.Interfaces;

internal interface IOrderRepository
{
    Task AddAsync(Order order);
    Task<List<Order>> ListAsync();
    Task SaveChangesAsync();
}
