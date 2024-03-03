using Ardalis.Result;
using MongoDB.Driver;

namespace RiverBooks.EmailSending;

internal class MongoDbOutBoxService : IOutboxService
{
    private readonly IMongoCollection<EmailOutboxEntity> _emailCollection;

    public MongoDbOutBoxService(IMongoCollection<EmailOutboxEntity> emailCollection)
    {
        _emailCollection = emailCollection;
    }

    /*
   * We could go and get all of the items that need to be processed, but just for demonstration purposes, we're going
   * to try do one by one so you can see them happening.
   */
    public async Task<Result<EmailOutboxEntity>> GetUnprocessedEmailEntity()
    {
        // look for entity with DateTimeUtcProcessed null
        var filter = Builders<EmailOutboxEntity>.Filter.Eq(entity => entity.DateTimeUtcProcessed, null);
        var unsentemailEntity = await _emailCollection.Find(filter).FirstOrDefaultAsync();

        if (unsentemailEntity is null) return Result.NotFound();

        return unsentemailEntity;
    }

    public async Task QueueEmailForSending(EmailOutboxEntity entity)
    {
        await _emailCollection.InsertOneAsync(entity);
    }
}
