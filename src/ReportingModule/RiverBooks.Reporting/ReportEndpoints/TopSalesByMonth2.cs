using FastEndpoints;

namespace RiverBooks.Reporting.ReportEndpoints;

internal class TopSalesByMonth2 : Endpoint<TopSalesByMonthRequest, TopSalesByMonthResponse>
{
    private readonly ISalesReportService _reportService;

    public TopSalesByMonth2(ISalesReportService reportService)
    {
        _reportService = reportService;
    }
    public override void Configure()
    {
        Get("/topsales2");
        AllowAnonymous(); // TODO: lock down
    }

    public override async Task HandleAsync(TopSalesByMonthRequest req, CancellationToken ct)
    {
        var report = _reportService.GetTopBooksByMonthReportAsync(req.Month, req.Year);
        var response = new TopSalesByMonthResponse()
        {
            Report = report
        };
        await SendAsync(response);
    }
}
