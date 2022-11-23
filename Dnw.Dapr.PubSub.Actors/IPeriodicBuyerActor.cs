using Dapr.Actors;
using JetBrains.Annotations;

namespace Dnw.Dapr.PubSub.Actors;

public interface IPeriodicBuyerActor : IActor
{
    [UsedImplicitly] Task Start();
    [UsedImplicitly] Task Stop();
}