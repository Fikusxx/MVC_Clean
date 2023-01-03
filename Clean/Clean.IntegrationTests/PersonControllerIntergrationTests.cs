using FluentAssertions;

namespace Clean.IntegrationTests;

public class PersonControllerIntergrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient client;

    public PersonControllerIntergrationTests(CustomWebApplicationFactory factory)
    {
        client = factory.CreateClient();
    }

    #region

    [Fact]
    public async void Index_ToReturnView()
    {
        var request = new HttpRequestMessage();
        request.Method = HttpMethod.Get;
        request.RequestUri = new Uri("http://localhost:5169/");
        HttpResponseMessage message = await client.SendAsync(request);

        message.Should().BeSuccessful();
    }

    #endregion
}
