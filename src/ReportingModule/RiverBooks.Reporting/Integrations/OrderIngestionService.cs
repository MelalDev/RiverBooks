using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace RiverBooks.Reporting.Integrations;

/*
* I didn't bother to create a separate interface for this service. It's only being used in this one place. If we needed to 
* mock it or if we were trying to decorate it using decorator pattern, we would want to create an interface that would have
* the one method that we're going to call on this service.
*/
public class OrderIngestionService
{
  private readonly ILogger<OrderIngestionService> _logger;
  private readonly string _connString;
  private static bool _ensureTableCreated = false;

  public OrderIngestionService(IConfiguration config,
    ILogger<OrderIngestionService> logger)
  {
    _connString = config.GetConnectionString("ReportingConnectionString")!;
    _logger = logger;
  }

  private async Task CreateTableAsync()
  {
    string sql = @"
IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = 'Reporting')
BEGIN
    EXEC('CREATE SCHEMA Reporting')
END

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'MonthlyBookSales' AND type = 'U')
BEGIN
    CREATE TABLE Reporting.MonthlyBookSales
    (
        BookId uniqueidentifier,
        Title NVARCHAR(255),
        Author NVARCHAR(255),
        Year INT,
        Month INT,
        UnitsSold INT,
        TotalSales DECIMAL(18, 2),
        PRIMARY KEY (BookId, Year, Month)
    );
END

";
    using var conn = new SqlConnection(_connString);
    _logger.LogInformation("Executing query: {sql}", sql);

    await conn.ExecuteAsync(sql);
    _ensureTableCreated = true;
  }

  /*
  * This is what we call every time for each order item that comes in on an order for one or more books, we're going to 
  * either update totals for the month in which that order took place for that book, or we're going to insert a new record,
  * all right? So this is what is colloquially called an upsert because it doesn't update or an insert, and so this is the
  * update (-- Update existing record) where it will increment the units sold or increment the total sales or and, both of
  * those things, for that book in that year and that month, or it'll just insert all those things (-- Insert new record).
  * So that's the crud of this ingestion service. It's got a bunch of hard-coded SQL, it's using Dapper instead of Entity
  * Framework. You could certainly do this with Entity Framework, I'm just showing that you have a lot of choices in how
  * you build these modules, and they don't have to all be the same.
  */
  public async Task AddOrUpdateMonthlyBookSalesAsync(BookSale sale)
  {
    if (!_ensureTableCreated) await CreateTableAsync();

    var sql = @"
    IF EXISTS (SELECT 1 FROM Reporting.MonthlyBookSales WHERE BookId = @BookId AND Year = @Year AND Month = @Month)
    BEGIN
        -- Update existing record
        UPDATE Reporting.MonthlyBookSales
        SET UnitsSold = UnitsSold + @UnitsSold, TotalSales = TotalSales + @TotalSales
        WHERE BookId = @BookId AND Year = @Year AND Month = @Month
    END
    ELSE
    BEGIN
        -- Insert new record
        INSERT INTO Reporting.MonthlyBookSales (BookId, Title, Author, Year, Month, UnitsSold, TotalSales)
        VALUES (@BookId, @Title, @Author, @Year, @Month, @UnitsSold, @TotalSales)
    END";

    using var conn = new SqlConnection(_connString);
    _logger.LogInformation("Executing query: {sql}", sql);
    await conn.ExecuteAsync(sql, new
    {
      sale.BookId,
      sale.Title,
      sale.Author,
      sale.Year,
      sale.Month,
      sale.UnitsSold,
      sale.TotalSales
    });
  }
}
