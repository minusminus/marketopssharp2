using RichardSzalay.MockHttp;
using System.Net;
using System.Text.Json;
using System.Text;

namespace MarketOps.Tests.Mocks;

/// <summary>
/// Mechanizm upraszczający wykonanie operacji na HttpClient.
/// </summary>
internal static class MockHttpClientManager
{
    public static HttpClient CreateHttpClient(MockHttpMessageHandler msgHandler, string uriString)
    {
        var client = msgHandler.ToHttpClient();
        client.BaseAddress = new Uri(uriString);
        return client;
    }

    public static MockHttpMessageHandler CreateMockHttpMessageHandler() =>
        new();

    public static void RespondWith(this MockHttpMessageHandler msgHandler, HttpStatusCode statusCode) =>
        msgHandler
            .When("*")
            .Respond(statusCode);

    public static void RespondWithSerializedJson(this MockHttpMessageHandler msgHandler,
        HttpStatusCode statusCode, object content) =>
        msgHandler
            .When("*")
            .Respond(statusCode, new StringContent(JsonSerializer.Serialize(content), Encoding.UTF8, "application/json"));

    public static void RespondWithSerializedJson(this MockHttpMessageHandler msgHandler, object content) =>
        msgHandler
            .When("*")
            .Respond("application/json", JsonSerializer.Serialize(content));
}