using RabbitMQ.Client;

namespace Khaos.Avalanche.RabbitMq;

public class RabbitMqSink : Sink<MessageToPublish>
{
    private readonly IModel _channel;
    private readonly IBasicProperties _defaultMessageProperties;

    private readonly bool _confirmEveryMessage;

    public RabbitMqSink(IModel channel, bool confirmEveryMessage = false)
    {
        _channel = channel;
        _defaultMessageProperties = _channel.CreateBasicProperties();
        _confirmEveryMessage = confirmEveryMessage;
    }

    protected override Task SinkElement(MessageToPublish element, CancellationToken cancellationToken)
    {
        _channel.BasicPublish(
            element.ExchangeName,
            element.RoutingKey,
            _defaultMessageProperties,
            element.Body);

        if (_confirmEveryMessage)
        {
            _channel.WaitForConfirmsOrDie();
        }

        return Task.CompletedTask;
    }
}