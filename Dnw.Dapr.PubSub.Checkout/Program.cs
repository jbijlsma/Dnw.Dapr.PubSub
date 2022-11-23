using System.Text.Json.Serialization;
using Dapr.Actors;
using Dapr.Actors.Client;
using Dapr.Client;
using Dnw.Dapr.PubSub.Actors;
using JetBrains.Annotations;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "Welcome");

// Publish an Order message that will be picked up by the order-service eventually
app.MapGet("/publish-order", async () =>
{
    using var client = new DaprClientBuilder().Build();
    var orderMessage = $"{DateTime.UtcNow:HH:mm:ss zz}: ordering using pubsub";
    await client.PublishEventAsync("pubsub", "orders", new Order(orderMessage));
    return Results.Ok("Order published");
});

// Invoke the order-service directly to place the order
app.MapGet("/order", async () =>
{
    using var client = new DaprClientBuilder().Build();
    var orderMessage = $"{DateTime.UtcNow:HH:mm:ss zz}: ordering with direct service call";
    var result = client.CreateInvokeMethodRequest(HttpMethod.Post, "order-processor", "orders", new Order(orderMessage));
    await client.InvokeMethodAsync(result);
    return Results.Ok("Order placed");
});

app.MapGet("/actor/start", async () =>
{
    var proxy = ActorProxy.Create<IPeriodicBuyerActor>(new ActorId("orderActor"), "PeriodicBuyerActor");
    await proxy.Start();
    
    return Results.Ok("Started placing orders every n seconds");
});

app.MapGet("/actor/stop", async () =>
{
    var proxy = ActorProxy.Create<IPeriodicBuyerActor>(new ActorId("orderActor"), "PeriodicBuyerActor");
    await proxy.Stop();
    
    return Results.Ok("Stopped actor placing");
});

app.MapGet("/actor/stop-all", async () =>
{
    var actorIds = new[]
    {
        "0f0e6eaf-d083-4256-a21b-388ae37ecc62",
        "5a5572a8-45b8-4337-800b-f6571ca2b51c"
    };

    foreach (var actorId in actorIds)
    {
        var proxy = ActorProxy.Create<IPeriodicBuyerActor>(new ActorId(actorId), "PeriodicBuyerActor");
        await proxy.Stop();
    }

    return Results.Ok("Stopped all actors placing orders");
});

app.Run();

public record Order([property: JsonPropertyName("message")][UsedImplicitly] string Message);