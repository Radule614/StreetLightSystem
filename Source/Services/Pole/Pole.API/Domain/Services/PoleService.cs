using Common;
using Common.Gprc;
using Grpc.Net.Client;
using NotificationProto;
using Pole.API.Domain.Entities;
using Pole.API.Domain.Exceptions;
using Pole.API.Domain.Specifications;
using Pole.API.Infrastructure.Data;

namespace Pole.API.Domain.Services;
/// <summary>
/// Pole Service used to manage complex domain logic.
/// </summary>
public class PoleService : IPoleService
{
    private readonly PoleRepository _poleRepository;
    /// <summary>
    /// Notification service channel used by notification grpc client
    /// </summary>
    private readonly GrpcChannel _notificationServiceChannel;
    /// <summary>
    /// Notification gprc client used to make grpc requests.
    /// </summary>
    private readonly NotificationGrpc.NotificationGrpcClient _notificationClient;
    /// <summary>
    /// Constructor used for dependency injection.
    /// </summary>
    [ActivatorUtilitiesConstructor]
    public PoleService(PoleRepository poleRepository, IConfiguration configuration, IChannelFactory factory)
    {
        _poleRepository = poleRepository;
        _notificationServiceChannel = factory.GetChannel(configuration[Constants.NotificationServiceAddress]!);
        _notificationClient = new NotificationGrpc.NotificationGrpcClient(_notificationServiceChannel);
    }
    /// <summary>
    /// Constructor used for testing.
    /// </summary>
    public PoleService(PoleRepository poleRepository, NotificationGrpc.NotificationGrpcClient notificationClient)
    {
        _poleRepository = poleRepository;
        _notificationClient = notificationClient;
        _notificationServiceChannel = null!;
    }

    ~PoleService()
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
        _notificationServiceChannel.Dispose();
    }

    public async Task<PoleEntity> Create(double latitude, double longitude)
    {
        Guid id = Guid.NewGuid();
        PoleEntity pole = new()
        {
            Id = id,
            Latitude = latitude,
            Longitude = longitude,
            Status = PoleStatus.Working
        };
        await _poleRepository.AddAsync(pole);
        return pole;
    }

    public async Task<PoleEntity> Update(Guid poleId, PoleStatus status, double latitude, double longitude)
    {
        ValidationExceptionBuilder exceptionBuilder = new();
        PoleEntity? pole = await _poleRepository.FirstOrDefaultAsync(new PoleSpecification(poleId));
        if (pole == null)
        {
            throw new PoleNotFoundException(poleId);
        }
        pole.Latitude = latitude;
        pole.Longitude = longitude;
        pole.Status = status;
        pole.ValidateData(exceptionBuilder);
        if (exceptionBuilder.HasErrors())
        {
            throw exceptionBuilder.Build();
        }
        await _poleRepository.UpdateAsync(pole);
        var notification = new BroadcastNotificationDto
        {
            Message = $"Pole status changed: {poleId} -> {pole.Status}",
            Action = "PoleStatusChanged"
        };
        await _notificationClient.BroadcastNotificationAsync(notification);
        return pole;
    }

    public async Task<PoleEntity> UpdateStatus(Guid poleId, PoleStatus status)
    {
        ValidationExceptionBuilder exceptionBuilder = new();
        PoleEntity? pole = await _poleRepository.FirstOrDefaultAsync(new PoleSpecification(poleId));
        if (pole == null)
        {
            throw new PoleNotFoundException(poleId);
        }
        pole.Status = status;
        pole.ValidateData(exceptionBuilder);
        if (exceptionBuilder.HasErrors())
        {
            throw exceptionBuilder.Build();
        }
        await _poleRepository.UpdateAsync(pole);

        var notification = new BroadcastNotificationDto
        {
            Message = $"Pole status changed: {poleId} -> {pole.Status}",
            Action = "PoleStatusChanged"
        };
        await _notificationClient.BroadcastNotificationAsync(notification);
        return pole;
    }
}
