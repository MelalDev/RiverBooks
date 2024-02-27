using Ardalis.Result;
using MediatR;
using Microsoft.Extensions.Logging;
using RiverBooks.OrderProcessing.Domain;
using RiverBooks.OrderProcessing.Interfaces;

namespace RiverBooks.OrderProcessing.Infrastructure;

/*
* So there's a few ways that we could do if the cached data not on Redis. We could just come in GetByIdAsync in 
* RedisOrderAddressCache.cs, we could say, you know what, if you don't find it, instead of just returning not found, let's go
* fetch it from the other system, right, but that is what leads to spaghetti code, right? That is not the responsibility of
* this particular service (RedisOrderAddressCache). This service is just the Redis order address cache. All it knows about
* is Redis and how to go fetch things and store things. If it's not in Redis, it's not its problem, so we want to create
* another type that is responsible for doing this extra work. This is called the read-through cache pattern, and so what
* we're going to do is try to read it from the cache. If it's not there, go fetch it from the source and store it in the cache
* in a different service.
*/
internal class ReadThroughOrderAddressCache : IOrderAddressCache
{
  private readonly RedisOrderAddressCache _redisCache;
  private readonly IMediator _mediator;
  private readonly ILogger<ReadThroughOrderAddressCache> _logger;

  public ReadThroughOrderAddressCache(RedisOrderAddressCache redisCache,
    IMediator mediator,
    ILogger<ReadThroughOrderAddressCache> logger)
  {
    _redisCache = redisCache;
    _mediator = mediator;
    _logger = logger;
  }

  public async Task<Result<OrderAddress>> GetByIdAsync(Guid id)
  {
    var result = await _redisCache.GetByIdAsync(id);
    if (result.IsSuccess) return result;

    if (result.Status == ResultStatus.NotFound)
    {
      // fetch data from source
      _logger.LogInformation("Address {id} not found; fetching from source.", id);
      var query = new Users.Contracts.UserAddressDetailsByIdQuery(id);

      var queryResult = await _mediator.Send(query);

      if (queryResult.IsSuccess)
      {
        var dto = queryResult.Value;
        var address = new Address(dto.Street1,
                                  dto.Street2,
                                  dto.City,
                                  dto.State,
                                  dto.PostalCode,
                                  dto.Country);
        var orderAddress = new OrderAddress(dto.AddressId, address);
        await StoreAsync(orderAddress);
        return orderAddress;
      }
    }
    return Result.NotFound();
  }

  public Task<Result> StoreAsync(OrderAddress orderAddress)
  {
    return _redisCache.StoreAsync(orderAddress);
  }
}
