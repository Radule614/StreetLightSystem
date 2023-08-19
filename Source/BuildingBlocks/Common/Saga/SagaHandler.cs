using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Common.Saga;
public abstract class SagaHandler<THandler, TCommand, TReply, TCommandType, TReplyType> : IDisposable, IHostedService
    where TCommand : ICommand<TCommandType>
    where TReply : IReply<TReplyType>
{
    protected readonly IServiceProvider _serviceProvider;
    protected readonly ILogger<THandler> Logger;
    private readonly IConnection _eventQueueConnection;
    private readonly IModel _eventQueueChannel;
    private readonly string _queueName;
    protected SagaHandler(IConfiguration configuration, ILogger<THandler> logger, IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        Logger = logger;
        var split = configuration[Constants.EventQueueAddress]?.Split(":")!;
        var connectionFactory = new ConnectionFactory
        {
            HostName = split[0],
            Port = int.Parse(split[1])
        };
        _eventQueueConnection = connectionFactory.CreateConnection();
        _eventQueueChannel = _eventQueueConnection.CreateModel();
        _queueName = _eventQueueChannel.QueueDeclare().QueueName;
        _eventQueueChannel.QueueBind(queue: _queueName, exchange: typeof(TCommand).Name, routingKey: string.Empty);
        _eventQueueChannel.QueueDeclare(typeof(TReply).Name, durable: false, exclusive: false, autoDelete: false, arguments: null);
    }

    ~SagaHandler()
    {
        Dispose(false);
    }

    public virtual void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposing) return;
        _eventQueueChannel.Dispose();
        _eventQueueConnection.Dispose();
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        HandleSagaCommands();
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        Dispose();
        return Task.CompletedTask;
    }

    protected void HandleSagaCommands()
    {
        var consumer = new EventingBasicConsumer(_eventQueueChannel);
        consumer.Received += async (model, args) =>
        {
            var body = args.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            Logger.LogInformation("Received command");
            var command = JsonSerializer.Deserialize<TCommand>(message);
            if (command == null) return;

            Logger.LogInformation("Command type: " + command.Type);
            var reply = await HandleCommand(command);

            if (reply.Type!.Equals(reply.UnknownType)) return;
            body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(reply));
            _eventQueueChannel.BasicPublish(exchange: "", routingKey: typeof(TReply).Name, basicProperties: null, body: body);
            Logger.LogInformation("Published reply type: " + reply.Type);
        };
        _eventQueueChannel.BasicConsume(queue: _queueName, autoAck: true, consumer: consumer, consumerTag: nameof(THandler));
    }
    protected abstract Task<TReply> HandleCommand(TCommand command);
}
