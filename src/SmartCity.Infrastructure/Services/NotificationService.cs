using Microsoft.AspNetCore.SignalR;
using SmartCity.Application.Common.Interfaces;

namespace SmartCity.Infrastructure.Services;

public class NotificationService : INotificationService
{
    private readonly IHubContext<NotificationHub> _hubContext;

    public NotificationService(IHubContext<NotificationHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task SendNotificationAsync(string userId, string message)
    {
        await _hubContext.Clients.Group(userId).SendAsync("ReceiveNotification", message);
    }

    public async Task BroadcastNotificationAsync(string message)
    {
        await _hubContext.Clients.All.SendAsync("ReceiveNotification", message);
    }
}
