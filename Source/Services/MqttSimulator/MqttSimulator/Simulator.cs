using Common;
using Common.Gprc;
using MQTTnet.Client;
using MQTTnet;
using PoleProto;
using MQTTnet.Protocol;
using Grpc.Net.Client;

namespace MqttSimulator;

/// <summary>
/// Simulator hosted service that's being run when application runs.
/// </summary>
public class Simulator : IHostedService, IDisposable
{
    /// <summary>
    /// Random number generator that's not supposed to be created each time random number is needed.
    /// </summary>
    private static readonly Random Random = new();
    /// <summary>
    /// Default logger class used to log information to console.
    /// </summary>
    private readonly ILogger<Simulator> _logger;
    /// <summary>
    /// Host address of the mqtt broker.
    /// </summary>
    private readonly string _mqttHost;
    /// <summary>
    /// Port of the mqtt broker.
    /// </summary>
    private readonly int _mqttPort;
    /// <summary>
    /// Pole service channel used by pole grpc client
    /// </summary>
    private readonly GrpcChannel _poleServiceChannel;
    /// <summary>
    /// Pole gprc client used to make grpc requests.
    /// </summary>
    private readonly PoleGrpc.PoleGrpcClient _poleClient;
    /// <summary>
    /// Mqtt client reference.
    /// </summary>
    private readonly IMqttClient _mqttClient;

    public Simulator(IConfiguration configuration, ILogger<Simulator> logger, IChannelFactory factory)
    {
        _logger = logger;

        var mqttAddressSplit = configuration[Constants.MqttBrokerAddress]!.Split(":");
        _mqttHost = mqttAddressSplit[0];
        _mqttPort = int.Parse(mqttAddressSplit[1]);

        _poleServiceChannel = factory.GetChannel(configuration[Constants.PoleServiceAddress]!);
        _poleClient = new PoleGrpc.PoleGrpcClient(_poleServiceChannel);

        var mqttFactory = new MqttFactory();
        _mqttClient = mqttFactory.CreateMqttClient();
    }

    ~Simulator()
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
        _poleServiceChannel.Dispose();
        _mqttClient.Dispose();
    }

    /// <inheritdoc />
    public Task StartAsync(CancellationToken cancellationToken)
    {
        RunSimulation(cancellationToken);
        return Task.CompletedTask;
    }
    /// <inheritdoc />
    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// Method connects to the mqtt broker and sends id of the pole that's to become broken.
    /// </summary>
    /// <param name="cancellationToken">Propagates information the the operation should cancel.</param>
    private async void RunSimulation(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            Thread.Sleep(12000);
            var pole = await GetRandomWorkingPole();
            if (pole == null)
            {
                continue;
            }

            var mqttClientOptions = new MqttClientOptionsBuilder()
                .WithClientId(Guid.NewGuid().ToString())
                .WithTcpServer(_mqttHost, _mqttPort)
                .WithCleanSession()
                .Build();

            await _mqttClient.ConnectAsync(mqttClientOptions, CancellationToken.None);

            var applicationMessage = new MqttApplicationMessageBuilder()
                .WithTopic("pole/broken")
                .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
                .WithPayload(pole.Id)
                .Build();

            await _mqttClient.PublishAsync(applicationMessage, CancellationToken.None);
            _logger.LogInformation("Pole broke: " + pole.Id);

            await _mqttClient.DisconnectAsync(cancellationToken: cancellationToken);
        }
    }

    /// <summary>
    /// Method used by simulation in order to fetch a random working pole.
    /// </summary>
    /// <returns>Random working pole object.</returns>
    private async Task<PoleDTO?> GetRandomWorkingPole()
    {
        var allPoles = await _poleClient.UnsecuredGetAllAsync(new Empty());
        List<PoleDTO> workingPoles = allPoles.Data.Where(pole => pole.Status == 0).ToList();
        if (workingPoles.Count == 0)
        {
            return null;
        }
        var index = Random.Next(workingPoles.Count);
        return workingPoles[index];
    }
}
