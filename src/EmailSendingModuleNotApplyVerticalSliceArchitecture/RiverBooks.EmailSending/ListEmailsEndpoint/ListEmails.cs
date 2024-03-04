using FastEndpoints;
using MongoDB.Driver;

namespace RiverBooks.EmailSending.ListEmailsEndpoint;

/*
* It would be useful to also be able to see what's in the queue, what's in the outbox. So for that, let's create another
* API endpoint (ListEmails).
*/
internal class ListEmails : EndpointWithoutRequest<ListEmailsResponse>
{
    /*
    * In here, normally we'd want to use a use case and have a query and have all of that. But again, for the sake of keeping
    * this simple, we're just going to directly reference the mongo collection.
    */
    private readonly IMongoCollection<EmailOutboxEntity> _emailCollection;

    public ListEmails(IMongoCollection<EmailOutboxEntity> emailCollection)
    {
        _emailCollection = emailCollection;
    }

    public override void Configure()
    {
        Get("/emails");

        //And just because I don't want to have to log in every time we check this, I'm going to make it allow anonymous.
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        //TODO: implement paging
        var filter = Builders<EmailOutboxEntity>.Filter.Empty;
        var emailEntities = await _emailCollection.Find(filter).ToListAsync();

        var response = new ListEmailsResponse
        {
            Count = emailEntities.Count,
            Emails = emailEntities
        };

        Response = response;
    }
}

public class ListEmailsResponse
{
    public int Count { get; set; }

    /*
    * We're just going to take the entities directly. Normally, you'd want to convert these into DTOs, but just for the sake
    * of simplicity, we'll keep it like this.
    */
    public List<EmailOutboxEntity> Emails { get; internal set; } = new();
}