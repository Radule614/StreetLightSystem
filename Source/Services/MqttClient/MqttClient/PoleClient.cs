using System.Text;
using Common;
using Common.Gprc;
using MQTTnet.Client;
using MQTTnet;
using PoleProto;
using Grpc.Net.Client;

namespace MqttClient;

/// <summary>
/// Pole client hosted service that's being run when application runs.
/// </summary>
public class PoleClient : IHostedService
{
    /// <summary>
    /// Default logger class used to log information to console.
    /// </summary>
    private readonly ILogger<PoleClient> _logger;
    /// <summary>
    /// Host address of the mqtt broker.
    /// </summary>
    private readonly string _mqttHost;
    /// <summary>
    /// Port of the mqtt broker.
    /// </summary>
    private readonly int _mqttPort;
    /// <summary>
    /// Property used to store mqttClient in order to dispose it later on when subscription is supposed to finish.
    /// </summary>
    private readonly IMqttClient _mqttClient;
    /// <summary>
    /// Pole service channel used by pole grpc client
    /// </summary>
    private readonly GrpcChannel _poleServiceChannel;
    /// <summary>
    /// Pole gprc client used to make grpc requests.
    /// </summary>
    private readonly PoleGrpc.PoleGrpcClient _poleClient;

    public PoleClient(IConfiguration configuration, ILogger<PoleClient> logger, IChannelFactory factory)
    {
        _logger = logger;

        var mqttAddressSplit = configuration["MQTT_BROKER_ADDRESS"]!.Split(":");
        _mqttHost = mqttAddressSplit[0];
        _mqttPort = int.Parse(mqttAddressSplit[1]);

        _poleServiceChannel = factory.GetChannel(configuration[Constants.PoleServiceAddress]!);
        _poleClient = new PoleGrpc.PoleGrpcClient(_poleServiceChannel);

        var mqttFactory = new MqttFactory();
        _mqttClient = mqttFactory.CreateMqttClient();
    }

    /// <inheritdoc />
    public Task StartAsync(CancellationToken cancellationToken)
    {
        SubscribeToPoleChanges(cancellationToken);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await _mqttClient.DisconnectAsync(cancellationToken: cancellationToken);
        _mqttClient.Dispose();
        _poleServiceChannel.Dispose();
    }

    /// <summary>
    /// Method connects to the mqtt broker and listens to pole status changes.
    /// </summary>
    /// <param name="cancellationToken">Propagates information the the operation should cancel.</param>
    private async void SubscribeToPoleChanges(CancellationToken cancellationToken)
    {
        var mqttClientOptions = new MqttClientOptionsBuilder().WithTcpServer(_mqttHost, _mqttPort).Build();
        _mqttClient.ConnectedAsync += async _ =>
        {
            var topic = new MqttTopicFilterBuilder().WithTopic("pole/broken").Build();
            await _mqttClient.SubscribeAsync(topic, cancellationToken);
        };
        _mqttClient.ApplicationMessageReceivedAsync += async messageReceivedEventArgs =>
        {
            var poleId = Encoding.UTF8.GetString(messageReceivedEventArgs.ApplicationMessage.PayloadSegment);
            _logger.LogInformation($"Pole client received broken pole id': {poleId}");
            await UpdatePoleStatus(poleId);
        };
        await _mqttClient.ConnectAsync(mqttClientOptions, cancellationToken);
    }

    /// <summary>
    /// Method used by subscription method in order to update pole's status.
    /// </summary>
    /// <param name="poleId">Id of the pole that's to be updated.</param>
    /// <returns>Empty task.</returns>
    private async Task UpdatePoleStatus(string poleId)
    {
        await _poleClient.UpdateStatusAsync(new UpdateStatusDTO { Id = poleId, Status = 1 });
    }
}
