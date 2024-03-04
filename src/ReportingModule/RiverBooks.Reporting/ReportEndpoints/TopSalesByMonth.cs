using FastEndpoints;

namespace RiverBooks.Reporting.ReportEndpoints;


internal record TopSalesByMonthRequest(int Year, int Month);
internal record TopSalesByMonthResponse();

internal class TopSalesByMonth : Endpoint<TopSalesByMonthRequest, TopSalesByMonthResponse>
{
    public override void Configure()
    {
        Get("/topsales");
        AllowAnonymous(); // TODO: lock down
    }

    public override async Task HandleAsync(TopSalesByMonthRequest req, CancellationToken ct)
    {
        var response = new TopSalesByMonthResponse();
        await SendAsync(response);
    }
}