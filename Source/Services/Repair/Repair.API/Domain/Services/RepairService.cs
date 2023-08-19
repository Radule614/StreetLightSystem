using Common.Saga.Repair;
using Common.Saga.Repair.Dto;
using RabbitMQ.Client;
using System.Text.Json;
using System.Text;
using Repair.API.Domain.Entities;
using Repair.API.Domain.Exceptions;
using Repair.API.Domain.Specifications;
using Repair.API.Infrastructure.Data;
using RepairProto;
using Constants = Common.Constants;
using Grpc.Net.Client;
using TeamProto;
using System.Linq;
using Common.Gprc;
using Repair.API.Domain.Utility;

namespace Repair.API.Domain.Services;

/// <summary>
/// Repair service used to manage complex domain logic.
/// </summary>
public class RepairService : IRepairService
{
    private readonly ILogger<RepairService> _logger;
    private readonly RepairRepository _repairRepository;
    private readonly IConnection _eventQueueConnection;
    private readonly IModel _eventQueueChannel;
    /// <summary>
    /// Team service channel used by team grpc client
    /// </summary>
    private readonly GrpcChannel _teamServiceChannel;
    /// <summary>
    /// Team gprc client used to make grpc requests.
    /// </summary>
    private readonly TeamGrpc.TeamGrpcClient _teamClient;

    public RepairService(RepairRepository repairRepository, IConfiguration configuration, ILogger<RepairService> logger, IChannelFactory channelFactory)
    {
        _logger = logger;
        _repairRepository = repairRepository;
        _teamServiceChannel = channelFactory.GetChannel(configuration[Constants.TeamServiceAddress]!);
        _teamClient = new TeamGrpc.TeamGrpcClient(_teamServiceChannel);
        var split = configuration[Constants.EventQueueAddress]?.Split(":")!;
        var factory = new ConnectionFactory
        {
            HostName = split[0],
            Port = int.Parse(split[1])
        };
        _eventQueueConnection = factory.CreateConnection();
        _eventQueueChannel = _eventQueueConnection.CreateModel();
    }

    ~RepairService()
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
        _teamServiceChannel.Dispose();
    }

    public async Task<RepairEntity> Create(Guid poleId)
    {
        RepairEntity repair = new()
        {
            Id = Guid.NewGuid(),
            PoleId = poleId,
            TeamId = null,
            StartDate = DateTime.UtcNow,
            EndDate = null,
            IsSuccessful = false
        };
        return await _repairRepository.AddAsync(repair);
    }

    public async Task<RepairEntity> EndRepair(Guid repairId, bool isSuccessful)
    {
        var repair = await _repairRepository.FirstOrDefaultAsync(new RepairSpecification(repairId));
        if (repair == null)
        {
            throw new RepairNotFoundException(repairId);
        }
        repair.IsSuccessful = isSuccessful;
        repair.EndDate = DateTime.UtcNow;
        await _repairRepository.UpdateAsync(repair);
        return repair;
    }

    public async Task<ICollection<RepairDto>> GetPoleRepairHistory(Guid poleId)
    {
        ICollection<RepairEntity> history = await _repairRepository.ListAsync(new PoleSpecification(poleId));
        var teamIds = new IdList();
        teamIds.Ids.AddRange(history.Select(r => r.TeamId.ToString()));
        var teams = await _teamClient.GetByIdListAsync(teamIds);
        return CustomConvert.HistoryToDto(history, teams.Data);
    }

    public void BeginStartRepairTransaction(RepairData data, Guid userId)
    {
        var command = new StartRepairCommand
        {
            Type = StartRepairCommandType.StartRepair,
            RepairData = data,
            UserId = userId
        };
        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(command));
        _eventQueueChannel.BasicPublish(exchange: nameof(StartRepairCommand), routingKey: string.Empty, basicProperties: null, body: body);
        _logger.LogInformation("Published command to begin the start repair saga");
    }

    public void BeginEndRepairTransaction(Guid repairId, Guid userId, bool success)
    {
        var command = new EndRepairCommand
        {
            Type = EndRepairCommandType.EndRepair,
            UserId = userId,
            RepairData = new RepairData { Id = repairId, IsSuccessful = success }
        };
        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(command));
        _eventQueueChannel.BasicPublish(exchange: nameof(EndRepairCommand), routingKey: string.Empty, basicProperties: null, body: body);
        _logger.LogInformation("Published command to begin the end repair saga");
    }
}
