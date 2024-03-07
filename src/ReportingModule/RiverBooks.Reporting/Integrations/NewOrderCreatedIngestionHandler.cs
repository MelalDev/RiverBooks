using MediatR;
using Microsoft.Extensions.Logging;
using RiverBooks.Books.Contracts;
using RiverBooks.OrderProcessing.Contracts;

namespace RiverBooks.Reporting.Integrations;

/*
* I'm going to call this new class an ingestion handler because it's all about data ingestion. It's taking data from other
* systems, other modules in this case, and bringing into the reporting database.
*/
internal class NewOrderCreatedIngestionHandler : INotificationHandler<OrderCreatedIntegrationEvent>
{
    private readonly ILogger<NewOrderCreatedIngestionHandler> _logger;
    private readonly OrderIngestionService _orderIngestionService;
    private readonly IMediator _mediator;

    public NewOrderCreatedIngestionHandler(ILogger<NewOrderCreatedIngestionHandler> logger,
      OrderIngestionService orderIngestionService,
      IMediator mediator)
    {
        _logger = logger;
        _orderIngestionService = orderIngestionService;
        _mediator = mediator;
    }

    public async Task Handle(OrderCreatedIntegrationEvent notification,
      CancellationToken ct)
    {
        _logger.LogInformation("Handling order created event to populate reporting database...");

        var orderItems = notification.OrderDetails.OrderItems;
        int year = notification.OrderDetails.DateCreated.Year;
        int month = notification.OrderDetails.DateCreated.Month;

        foreach (var item in orderItems)
        {
            // look up book details to get author and title
            // TODO: Implement Materialized View or other cache
            /*
            * Now the one thing to note that about this is that the details that we're getting from the Order include a 
            * description, but they don't actually include a separate author and a separate title because when we decided
            * how to store information in the shopping cart, we used description and munged those things together with a
            * string concatenation, and I could just try and de-concatenate them and look for a space and then the word 'by'
            * and another space, because that's what we said, it's title by author, but that seemed really hacky. So intead,
            * I'm actually going to use this BookDetailsQuery again, we're already using it everywhere, to go and get the actual
            * information about the book, because we have the ID, and pull out the author and title. Now, we would probably want
            * to put this in some type of materialized view or cache so that we're not having to make this query over and over
            * and over again, because it's not data that's going to change very often, and this at the moment is going to
            * actually hit the database every time to fetch that title and author information, but leaving efficiency aside
            * for a momment, and that's an easily solved problem, I need to just create a reference to the books module so that
            * this can work.
            */
            var bookDetailsQuery = new BookDetailsQuery(item.BookId);
            var result = await _mediator.Send(bookDetailsQuery);

            if (!result.IsSuccess)
            {
                _logger.LogWarning("Issue loading book details for {id}", item.BookId);
                continue;
            }

            string author = result.Value.Author;
            string title = result.Value.Title;

            var sale = new BookSale
            {
                Author = author,
                BookId = item.BookId,
                Month = month,
                Title = title,
                Year = year,
                TotalSales = item.Quantity * item.UnitPrice,
                UnitsSold = item.Quantity
            };

            await _orderIngestionService.AddOrUpdateMonthlyBookSalesAsync(sale);
        }
    }
}