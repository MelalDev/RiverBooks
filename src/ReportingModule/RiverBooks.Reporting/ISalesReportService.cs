using System.Globalization;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace RiverBooks.Reporting;

internal interface ISalesReportService
{
    Task<TopBooksByMonthReport> GetTopBooksByMonthReportAsync(int month, int year);
}

internal class DefaultSalesReportService : ISalesReportService
{
    private readonly ILogger<DefaultSalesReportService> _logger;
    private readonly string _connString;

    public DefaultSalesReportService(IConfiguration config,
      ILogger<DefaultSalesReportService> logger)
    {
        _connString = config.GetConnectionString("ReportingConnectionString")!;
        _logger = logger;
    }

    public async Task<TopBooksByMonthReport> GetTopBooksByMonthReportAsync(int month, int year)
    {
        /*
        * Notice that it's going to against the Reporting.MonthlyBookSales, that's the thing that we just created an we're
        * inserting or updating to, and we just need to do an order by (ORDER BY TotalSales DESC), we don't need to do any joins
        * or any of that other stuff, we don't need to do any aggregation, there's no summing happening here 
        * (UnitsSold as Units, TotalSales as Sales) (check TopSellingBooksReportService.cs to compare), right, the aggregation 
        * is happening as we add the data with those upserts, it's a running total, and so we will get accurate data based on 
        * all the events that a re coming in up to however many events have been processed, right, so this won't necessarily be 
        * accurate to the instant, to like the very latest sale that was purchased, but it will be very quickly accurate, and 
        * for a month in the past, which is typically when you're going to run this report, it will be accurate assuming that 
        * all the events for that month have been tallied (Remember there is no queue! These events are handled before the 
        * checkout request finishes)
        * 
        */
        string sql = @"
select BookId, Title, Author, UnitsSold as Units, TotalSales as Sales
from Reporting.MonthlyBookSales
where Month = @month and Year = @year
ORDER BY TotalSales DESC
";
        using var conn = new SqlConnection(_connString);
        _logger.LogInformation("Executing query: {sql}", sql);
        var results = (await conn.QueryAsync<BookSalesResult>(sql, new { month, year }))
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
