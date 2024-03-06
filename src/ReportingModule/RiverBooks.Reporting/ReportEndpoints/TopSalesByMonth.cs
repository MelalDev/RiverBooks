using FastEndpoints;
using Microsoft.AspNetCore.Mvc;

namespace RiverBooks.Reporting.ReportEndpoints;

internal class TopSalesByMonthRequest
{
    [FromQuery]
    public int Month { get; set; }
    [FromQuery]
    public int Year { get; set; }
}

internal class TopSalesByMonthResponse
{
    public TopBooksByMonthReport Report { get; set; } = default!;
}

internal class TopSalesByMonth : Endpoint<TopSalesByMonthRequest, TopSalesByMonthResponse>
{
    private readonly ITopSellingBooksReportService _reportService;

    public TopSalesByMonth(ITopSellingBooksReportService reportService)
    {
        _reportService = reportService;
    }
    public override void Configure()
    {
        Get("/topsales");
        AllowAnonymous(); // TODO: lock down
    }

    public override async Task HandleAsync(TopSalesByMonthRequest req, CancellationToken ct)
    {
        var report = _reportService.ReachInSqlQuery(req.Month, req.Year);
        var response = new TopSalesByMonthResponse()
        {
            Report = report
        };
        await SendAsync(response);
    }
}