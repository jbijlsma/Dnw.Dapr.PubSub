using System.Text.Json.Serialization;
using Dapr.Client;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "Welcome");

app.MapGet("/publish-message", async () =>
{
    using var client = new DaprClientBuilder().Build();
    await client.PublishEventAsync("pubsub", "orders", new Order(20));
    return "Message published";
});

app.Run();

public record Order([property: JsonPropertyName("orderId")] int OrderId);