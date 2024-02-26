using FastEndpoints.Testing;
using Xunit.Abstractions;

namespace RiverBooks.Books.Tests.Endpoints;

/*
* XUnit has a notion of test fixtures, which are things that sort of wrap your tests. So we're going to use this along
* with some of the fast-endpoints testing features that are in that NuGet package to help us with our testing.
*/
/*
* (TestFixture<Program>) Now we need this to reference the program class that's in our web project, and for that to work,
* we need to add a partial class to that program. So we're going to jump down here to program.cs (RiverBooks.Web)
* and at the very bottom, we're just going to add this line (please check Program.cs at the verrry bottom).
*/
/*
* we add reference RiverBooks.Web. You may wonder why are we referencing web when we're trying to test books. The reason
* is that our host is what's going to host the Api, and since we're testing those Apis, we need to test it through the 
* host.
*/
public class Fixture(IMessageSink messageSink) : TestFixture<Program>(messageSink)
{
    /*
    * Now, these fixtures basically have a setup and a teardown, and we just use these methods to set up any dependencies
    * that our tests have. The only thing we're going to use them for is to set up our default client that's going to be
    * used to hit the Apis.
    */
    protected override Task SetupAsync()
    {
        Client = CreateClient();

        return Task.CompletedTask;
    }

    protected override Task TearDownAsync()
    {
        Client.Dispose();

        return base.TearDownAsync();
    }
}