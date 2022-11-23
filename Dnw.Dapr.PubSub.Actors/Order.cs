using System.Text.Json.Serialization;
using JetBrains.Annotations;

namespace Dnw.Dapr.PubSub.Actors;

public record Order([property: JsonPropertyName("message")][UsedImplicitly] string Message);