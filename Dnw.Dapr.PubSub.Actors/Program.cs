using Dnw.Dapr.PubSub.Actors;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddActors(options =>
{
    options.Actors.RegisterActor<PeriodicBuyerActor>();
});

var app = builder.Build();

app.MapGet("/", () => "All actors say hello!");

app.MapActorsHandlers();

app.Run();
