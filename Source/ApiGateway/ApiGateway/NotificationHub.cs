using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;

namespace ApiGateway;

public class NotificationHub : Hub
{
    private static readonly ConcurrentDictionary<string, IList<string>?> ConnectionDictionary = new();
    private readonly ILogger<NotificationHub> _logger;

    public NotificationHub(ILogger<NotificationHub> logger)
    {
        _logger = logger;
    }
    public static bool IsConnectionActive(string userId)
    {
        return ConnectionDictionary.ContainsKey(userId) && ConnectionDictionary[userId]?.Count != 0;
    }
    private async Task AddToGroup(string userId)
    {
        if (!ConnectionDictionary.ContainsKey(userId))
        {
            ConnectionDictionary.TryAdd(userId, new List<string>());
        }
        ConnectionDictionary[userId]?.Add(Context.ConnectionId);
        await Groups.AddToGroupAsync(Context.ConnectionId, userId);
    }

    private async Task RemoveFromGroup(string? userId)
    {
        if (ConnectionDictionary.TryGetValue(userId ?? "", out var value))
        {
            value?.Remove(Context.ConnectionId);
        }
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, userId ?? "");
    }

    public override async Task OnConnectedAsync()
    {
        var userId = Context.GetHttpContext()?.Request.Query["userId"].ToString();
        if (!Guid.TryParse(userId, out _)) return;
        await AddToGroup(userId);
        await base.OnConnectedAsync();
        _logger.LogInformation($"User connected to the notification hub: {userId}");
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = Context.GetHttpContext()?.Request.Query["userId"].ToString();
        await RemoveFromGroup(userId);
        await base.OnDisconnectedAsync(exception);
        _logger.LogInformation($"User disconnected from the notification hub: {userId}");
    }
}