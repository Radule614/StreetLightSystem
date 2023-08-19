using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace Common.Saga;
public abstract class SagaOrchestrator<TOrchestrator, TCommand, TReply, TCommandType, TReplyType> : IDisposable, IHostedService
    where TCommand : ICommand<TCommandType>
    where TReply : IReply<TReplyType>
{
    protected readonly ILogger<TOrchestrator> Logger;
    private readonly IConnection _connection;
    private readonly IModel _channel;

    protected SagaOrchestrator(IConfiguration configuration, ILogger<TOrchestrator> logger)
    {
        Logger = logger;
        var split = configuration[Constants.EventQueueAddress]?.Split(":")!;
        var factory = new ConnectionFactory
        {
            HostName = split[0],
            Port = int.Parse(split[1])
        };
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
        _channel.ExchangeDeclare(exchange: typeof(TCommand).Name, type: ExchangeType.Fanout);
        _channel.QueueDeclare(typeof(TReply).Name, durable: false, exclusive: false, autoDelete: false, arguments: null);
    }

    ~SagaOrchestrator()
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
        _channel.Dispose();
        _connection.Dispose();
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        HandleSagaReplies();
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        Dispose();
        return Task.CompletedTask;
    }

    public void HandleSagaReplies()
    {
        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += (model, args) =>
        {
            var body = args.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            Logger.LogInformation("Orchestrator received reply");
            var reply = JsonSerializer.Deserialize<TReply>(message);
            var command = GetNextCommand(reply);
            if (reply != null)
            {
                Logger.LogInformation("Reply type: " + reply.Type);
            }
            if (command.Type!.Equals(command.UnknownType)) return;
            body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(command));
            _channel.BasicPublish(exchange: typeof(TCommand).Name, routingKey: string.Empty, basicProperties: null, body: body);
            Logger.LogInformation("Orchestrator publishing new command: " + command.Type);
        };
        _channel.BasicConsume(queue: typeof(TReply).Name, autoAck: true, consumer: consumer, consumerTag: nameof(TOrchestrator));
    }

    protected abstract TCommand GetNextCommand(TReply? reply);
}
