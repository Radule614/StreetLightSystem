using AutoMapper;
using Common;
using Common.Gprc;
using Grpc.Core;
using Grpc.Net.Client;
using Notification.API.Domain.Entities;
using Notification.API.Domain.Specifications;
using Notification.API.Infrastructure.Data;
using NotificationProto;
using UserProto;

namespace Notification.API.Domain.Services;

public class NotificationService : INotificationService, IDisposable
{
    /// <summary>
    /// Api gateway channel used by api gateway client.
    /// </summary>
    private readonly GrpcChannel _gatewayChannel;
    /// <summary>
    /// Api gateway gprc client used to make grpc requests.
    /// </summary>
    private readonly GatewayProto.NotificationService.NotificationServiceClient _gatewayClient;
    /// <summary>
    /// User service channel used by user grpc client
    /// </summary>
    private readonly GrpcChannel _userServiceChannel;
    /// <summary>
    /// User gprc client used to make grpc requests.
    /// </summary>
    private readonly UserGrpc.UserGrpcClient _userClient;

    private readonly NotificationRepository _notificationRepository;
    private readonly ILogger<NotificationService> _logger;
    private readonly IMapper _mapper;
    public NotificationService(
        IConfiguration configuration,
        IChannelFactory factory, 
        NotificationRepository notificationRepository, 
        ILogger<NotificationService> logger,
        IMapper mapper)
    {
        _gatewayChannel = factory.GetChannel(configuration[Constants.ApiGatewayAddress]!);
        _gatewayClient = new GatewayProto.NotificationService.NotificationServiceClient(_gatewayChannel);
        _userServiceChannel = factory.GetChannel(configuration[Constants.UserServiceAddress]!);
        _userClient = new UserGrpc.UserGrpcClient(_userServiceChannel);
        _notificationRepository = notificationRepository;
        _logger = logger;
        _mapper = mapper;
    }

    ~NotificationService()
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
        _gatewayChannel.Dispose();
        _userServiceChannel.Dispose();
    }

    public async Task SendMessage(string message, Guid receiverId, Guid senderId, Sender senderData)
    {
        var notification = new NotificationEntity
        {
            Id = Guid.NewGuid(),
            Message = message,
            Received = false,
            ReceiverId = receiverId,
            SenderId = senderId,
            SentDate = DateTime.UtcNow,
            IsNew = true
        };
        var messageDto = _mapper.Map<MessageDto>(notification);
        messageDto.Sender = senderData;
        await _notificationRepository.AddAsync(notification);
        try
        {
            await _gatewayClient.SendMessageAsync(messageDto);
            notification.Received = true;
            notification.ReceivedDate = DateTime.UtcNow;
            await _notificationRepository.UpdateAsync(notification);
        }
        catch (RpcException ex)
        {
            var error = RpcError.ParseRpcErrorMessage(ex.Message);
            if (!error.StatusCode.Equals("FailedPrecondition"))
                throw;
            _logger.LogWarning(error.Detail);
        }
    }

    public async Task BroadcastNotification(BroadcastNotificationDto notification)
    {
        await _gatewayClient.BroadcastNotificationAsync(notification);
    }

    public async Task SendNotification(NotificationDto notification)
    {
        await _gatewayClient.SendNotificationAsync(notification);
    }

    public async Task<ICollection<MessageDto>> GetUserMessages(Guid userId)
    {
        var notificationEntities = await _notificationRepository.ListAsync(new UserSpecification(userId));
        var senderIds = new IdCollection();
        senderIds.Ids.AddRange(notificationEntities.Select(entity => entity.SenderId.ToString()));
        var senders = await _userClient.GetAllByIdsAsync(senderIds);
        var response = notificationEntities.Select(entity => ConvertMessageToDto(entity, senders.Data)).ToList();
        notificationEntities.ForEach(notification => notification.IsNew = false);
        await _notificationRepository.UpdateRangeAsync(notificationEntities);
        return response;
    }

    public async Task NotifyUnsentMessages(Guid userId)
    {
        var notificationEntities = await _notificationRepository.ListAsync(new UserUnsentSpecification(userId));
        var senderIds = new IdCollection();
        senderIds.Ids.AddRange(notificationEntities.Select(entity => entity.SenderId.ToString()));
        var senders = await _userClient.GetAllByIdsAsync(senderIds);
        var messageList = new MessageList();
        messageList.Messages.AddRange(notificationEntities.Select(entity => ConvertMessageToDto(entity, senders.Data)).ToList());
        try
        {
            await _gatewayClient.SendMessagesAsync(_mapper.Map<MessageList>(messageList));
            notificationEntities.ForEach(entity =>
            {
                entity.Received = true;
                entity.ReceivedDate = DateTime.UtcNow;
            });
            await _notificationRepository.UpdateRangeAsync(notificationEntities);
        }
        catch (RpcException ex)
        {
            var error = RpcError.ParseRpcErrorMessage(ex.Message);
            if (!error.StatusCode.Equals("FailedPrecondition"))
                throw;
            _logger.LogWarning(error.Detail);
        }
    }

    /// <summary>
    /// Helper method for converting message to dto while mapping sender data.
    /// </summary>
    /// <param name="message">Message data.</param>
    /// <param name="senders">List of senders.</param>
    /// <returns>Message dto</returns>
    private MessageDto ConvertMessageToDto(NotificationEntity message, IEnumerable<UserDto> senders)
    {
        var data = _mapper.Map<MessageDto>(message);
        data.Sender =
            _mapper.Map<Sender>(senders.FirstOrDefault(sender => sender.Id.Equals(message.SenderId.ToString())));
        return data;
    }
}
