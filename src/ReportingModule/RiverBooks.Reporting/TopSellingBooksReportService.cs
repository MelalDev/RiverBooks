using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Globalization;

namespace RiverBooks.Reporting;

/*
* This is our implementation of that interface (reachin-report anti-pattern). So it's going to take in a connection
* string and a logger, just so we can maybe see what's happening in this report. The connection string, we're just
* going to get from the order processing module. We're not going to create our own because this isn't really our
* data. So we're going to be honest here about the fact that we're just reaching into some other module and
* grabbing their internal data representation. And by the way, we're assuming that somehow that's also going to
* let us get to data from the books module because we just happen to know that that's currently how those modules
* databases are configured. If later on, the books module team decides that they want to put their data somewhere
* else, then obviously, this report is going to break.
*/
internal class TopSellingBooksReportService : ITopSellingBooksReportService
{
  private readonly ILogger<TopSellingBooksReportService> _logger;
  private readonly string _connString;

  public TopSellingBooksReportService(IConfiguration config,
    ILogger<TopSellingBooksReportService> logger)
  {
    _connString = config.GetConnectionString("OrderProcessingConnectionString")!;
    _logger = logger;
  }

  public TopBooksByMonthReport ReachInSqlQuery(int month, int year)
  {
    string sql = @"
select b.Id, b.Title, b.Author, sum(oi.Quantity) as Units, sum(oi.UnitPrice * oi.Quantity) as Sales
from Books.Books b 
	inner join OrderProcessing.OrderItem oi on b.Id = oi.BookId
	inner join OrderProcessing.Orders o on o.Id = oi.OrderId
where MONTH(o.DateCreated) = @month and YEAR(o.DateCreated) = @year
group by b.Id, b.Title, b.Author
ORDER BY Sales DESC
";
    using var conn = new SqlConnection(_connString);
    _logger.LogInformation("Executing query: {sql}", sql);
    var results = conn.Query<BookSalesResult>(sql, new { month, year })
      .ToList();

    var report = new TopBooksByMonthReport
    {
      Year = year,
      Month = month,
      MonthName = CultureInfo.GetCultureInfo("en-US").DateTimeFormat.GetMonthName(month),
      Results = results
    };

    return report;
  }
}
