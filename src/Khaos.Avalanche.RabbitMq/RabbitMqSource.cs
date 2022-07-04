using System.Collections.Concurrent;
using System.Runtime.CompilerServices;

using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Khaos.Avalanche.RabbitMq;

public class RabbitMqSource : Source<BasicDeliverEventArgs>
{
    private readonly EventingBasicConsumer _consumer;
    private readonly TimeSpan _endOfStreamTimeout = TimeSpan.Zero;

    private readonly ConcurrentQueue<BasicDeliverEventArgs> _buffer = new();

    public RabbitMqSource(IModel channel, string queueName, TimeSpan? endOfStreamTimeout = null)
    {
        _consumer = new EventingBasicConsumer(channel);

        if (endOfStreamTimeout is not null)
        {
            _endOfStreamTimeout = endOfStreamTimeout.Value;
        }
        
        _consumer.Received += ConsumerOnReceived;
        channel.BasicConsume(_consumer, queueName);
    }

    private void ConsumerOnReceived(object? sender, BasicDeliverEventArgs e)
    {
        _buffer.Enqueue(e);
        _consumer.Model.BasicAck(e.DeliveryTag, false);
    }

    protected override async IAsyncEnumerable<BasicDeliverEventArgs> GetEnumerable(
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var timedOut = false;
        var previouslyReceivedMessage = false;

        while (!timedOut)
        {
            bool receivedMessage;

            do
            {
                receivedMessage = _buffer.TryDequeue(out var ea);

                if (receivedMessage)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    yield return ea!;
                }

                timedOut = !receivedMessage && !previouslyReceivedMessage;
                previouslyReceivedMessage = receivedMessage;
            } while (receivedMessage);

            await Task.Delay(_endOfStreamTimeout, cancellationToken);
        }
    }
}