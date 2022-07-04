namespace Khaos.Avalanche.RabbitMq;

public record MessageToPublish(string ExchangeName, ReadOnlyMemory<byte> Body, string? RoutingKey = null);