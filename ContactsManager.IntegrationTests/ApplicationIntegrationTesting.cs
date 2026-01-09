using Fizzler.Systems.HtmlAgilityPack;
using FluentAssertions;
using HtmlAgilityPack;
using Xunit.Abstractions;

namespace ContactsManager.IntegrationTests;

public class ApplicationIntegrationTesting : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _httpClient;
    private readonly ITestOutputHelper _testOutput;
    public ApplicationIntegrationTesting(CustomWebApplicationFactory factory, ITestOutputHelper testOutput)
    {
        _httpClient = factory.CreateClient();
        _testOutput = testOutput;
    }

    [Fact]
    public async Task HomePage_SuccessfulStatusCode_SendingGetRequestToTheHomePage()
    {
        HttpResponseMessage responseMessage = await _httpClient.GetAsync("/Home/Index");

        responseMessage.IsSuccessStatusCode.Should().BeTrue();

        string responseHtmlContent = await responseMessage.Content.ReadAsStringAsync();

        HtmlDocument htmlDocument = new HtmlDocument();
        htmlDocument.LoadHtml(responseHtmlContent);

        HtmlNode document = htmlDocument.DocumentNode;
        document.QuerySelector("form").Should().NotBeNull();
    }
}
