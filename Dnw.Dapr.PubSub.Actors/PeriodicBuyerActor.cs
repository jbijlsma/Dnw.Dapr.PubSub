using Dapr.Actors.Runtime;
using Dapr.Client;
using JetBrains.Annotations;

namespace Dnw.Dapr.PubSub.Actors;

[UsedImplicitly]
public class PeriodicBuyerActor : Actor, IPeriodicBuyerActor, IRemindable
{
    private const string BuyReminder = "BuyReminder";
    private const int BuyIntervalInSeconds = 2;
    
    public async Task Start()
    {
        await RegisterBuyReminder();
    }

    public async Task Stop()
    {
        await UnregisterBuyReminder();
    }

    public PeriodicBuyerActor(ActorHost host) : base(host)
    {
    }

    public async Task ReceiveReminderAsync(string reminderName, byte[] state, TimeSpan dueTime, TimeSpan period)
    {
        // await UnregisterBuyReminder();

        try
        {
            using var client = new DaprClientBuilder().Build();
            var message = $"{DateTime.UtcNow:HH:mm:ss zz}: Actor {Id} ordered";
            await client.PublishEventAsync("pubsub", "orders", new Order(message));
        }
        finally
        {
            // await RegisterBuyReminder();
        }
    }

    private async Task UnregisterBuyReminder()
    {
        await UnregisterReminderAsync(BuyReminder);
    }

    private async Task RegisterBuyReminder()
    {
        await RegisterReminderAsync(
            BuyReminder, 
            Array.Empty<byte>(), 
            TimeSpan.FromMilliseconds(0), // -1 should work, but it gives an exception
            TimeSpan.FromSeconds(BuyIntervalInSeconds));
    }
}