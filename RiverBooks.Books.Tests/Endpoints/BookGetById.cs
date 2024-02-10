using FastEndpoints;
using FastEndpoints.Testing;
using FluentAssertions;
using RiverBooks.Books.BookEndpoints;
using Xunit.Abstractions;

namespace RiverBooks.Books.Tests.Endpoints;

public class BookGetById(Fixture fixture, ITestOutputHelper outputHelper) : TestClass<Fixture>(fixture, outputHelper)
{
    [Theory]
    [InlineData("0cf1a38e-3a12-4ee3-9d61-427f429969a1", "The Fellowship of the Ring")]
    [InlineData("28f9de3e-7584-4355-8b4e-cf301c159d30", "The Two Towers")]
    [InlineData("25b699ec-cd96-432a-b676-407f3ee90ebe", "The Return of the King")]
    public async Task ReturnExpextedBookGivenIdAsync(string validId, string expectedTitle)
    {
        Guid id = Guid.Parse(validId);
        var request = new GetBookByIdRequest { Id = id };
        var testResult = await Fixture.Client.GETAsync<GetById, GetBookByIdRequest, BookDto>(request);

        testResult.Response.EnsureSuccessStatusCode();
        testResult.Result.Title.Should().Be(expectedTitle);
    }
}
