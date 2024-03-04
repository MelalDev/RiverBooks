using Ardalis.Result;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace RiverBooks.EmailSending.EmailBackgroundService;

internal interface IGetEmailsFromOutboxService
{
    /*
 * We could go and get all of the items that need to be processed, but just for demonstration purposes, we're going
 * to try do one by one so you can see them happening.
 */
    Task<Result<EmailOutboxEntity>> GetUnprocessedEmailEntity();
}

internal class MongoDbGetEmailsFromOutboxOutBoxService : IGetEmailsFromOutboxService
{
    private readonly IMongoCollection<EmailOutboxEntity> _emailCollection;

    public MongoDbGetEmailsFromOutboxOutBoxService(IMongoCollection<EmailOutboxEntity> emailCollection)
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
}

internal class DefaultSendEmailsFromOutboxService : ISendEmailsFromOutboxService
{
    /*
    * Now, I don't love that we have some different levels of abstraction here, seeing as these two things are very
    * abstract (IOutboxService, ISendEmail), but this one is MongoDB specific (IMongoCollection<EmailOutboxEntity>), so if
    * this were production code I was building, I would probably put this behind a repository or some other more abstract
    * type of interface, but this should work for our needs right now.
    */
    private readonly IGetEmailsFromOutboxService _outboxService;
    private readonly ISendEmail _emailSender;
    private readonly IMongoCollection<EmailOutboxEntity> _emailEntityCollection;
    private readonly ILogger<DefaultSendEmailsFromOutboxService> _logger;

    public DefaultSendEmailsFromOutboxService(IGetEmailsFromOutboxService outboxService,
        ISendEmail emailSender,
        IMongoCollection<EmailOutboxEntity> emailEntityCollection,
        ILogger<DefaultSendEmailsFromOutboxService> logger)
    {
        _outboxService = outboxService;
        _emailSender = emailSender;
        _emailEntityCollection = emailEntityCollection;
        _logger = logger;
    }

    public async Task CheckForAndSendEmailsAsync()
    {
        try
        {
            var result = await _outboxService.GetUnprocessedEmailEntity();

            if (result.Status == ResultStatus.NotFound) return;

            var emailEntity = result.Value;

            // if this is not successful, it will throw exception, but the background worker apply try/catch. So we don't
            // necessarily apply try/catch here.
            await _emailSender.SendEmailAsync(emailEntity.To, emailEntity.From, emailEntity.Subject, emailEntity.Body);

            var updateFilter = Builders<EmailOutboxEntity>.Filter.Eq(x => x.Id, emailEntity.Id);
            var update = Builders<EmailOutboxEntity>.Update.Set("DateTimeUtcProcessed", DateTime.UtcNow);
            var updateResult = await _emailEntityCollection.UpdateOneAsync(updateFilter, update);
            _logger.LogInformation("Processed {result} email records.", updateResult.ModifiedCount);
        }
        finally
        {
            _logger.LogInformation("Sleeping ...");
        }
    }
}
