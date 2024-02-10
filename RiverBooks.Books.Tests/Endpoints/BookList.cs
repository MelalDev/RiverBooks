using FastEndpoints;
using FastEndpoints.Testing;
using FluentAssertions;
using RiverBooks.Books.BookEndpoints;
using Xunit.Abstractions;

namespace RiverBooks.Books.Tests.Endpoints;

/*
* Okay, so this is fairly complicated looking class definition, but basically, we're specifying the fixture that we're
* going to use, which is the one we just wrote. And we're also using the ITestOutputHelper, which is how you can write
* stuff out to see what's going on inside of an XUnit test.
*/
public class BookList(Fixture fixture, ITestOutputHelper outputHelper) : TestClass<Fixture>(fixture, outputHelper)
{

    /*
    * Now, in this test, we just want to verify that when we hit the list endpoint, we get back the three token books
    * that we expect. 
    */
    [Fact]
    public async Task ReturnsThreeBookAsync()
    {
        /*
        * And when you call this Client, there are helpers. The one that you want is typically going
        * to be in all caps, so we want the shoutingAtYouGetAsync, not the camelCaseGetAsync or pascalCaseGetAsync.
        */
        var testResult = await Fixture.Client.GETAsync<List, ListBooksResponse>();

        /*
        * Now this testResult type is going to have two parts to it. It has a response and it also has a result. The response
        * is literally the HTTP response, so we can ensure that it has a sucessfult status code, like a 200OK.
        */
        testResult.Response.EnsureSuccessStatusCode();

        /*
        * And then the result is the actual body that we get back, in this case, the listBooks response.
        */
        testResult.Result.Books.Count.Should().Be(3);
    }
}
