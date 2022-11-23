using Dapr.Actors;

// ReSharper disable once CheckNamespace
// This has to be in the namespace of the Dnw.Dapr.PubSub.Actors project
namespace Dnw.Dapr.PubSub.Actors;

public interface IPeriodicBuyerActor : IActor
{
    Task Start();
    Task Stop();
}