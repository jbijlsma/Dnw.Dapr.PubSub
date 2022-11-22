using System.Text.Json.Serialization;
using Dapr.Client;
using JetBrains.Annotations;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "Welcome");

// Publish an Order message that will be picked up by the order-service eventually
app.MapGet("/publish-order", async () =>
{
    using var client = new DaprClientBuilder().Build();
    await client.PublishEventAsync("pubsub", "orders", new Order(20));
    return Results.Ok("Order published");
});

// Invoke the order-service directly to place the order
app.MapGet("/order", async () =>
{
    using var client = new DaprClientBuilder().Build();
    var result = client.CreateInvokeMethodRequest(HttpMethod.Post, "order-processor", "orders", new Order(21));
    await client.InvokeMethodAsync(result);
    return Results.Ok("Order placed");
});

app.Run();

public record Order([property: JsonPropertyName("orderId")][UsedImplicitly] int OrderId);