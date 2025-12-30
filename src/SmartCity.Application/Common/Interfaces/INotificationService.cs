namespace SmartCity.Application.Common.Interfaces;

public interface INotificationService
{
    Task SendNotificationAsync(string userId, string message);
    Task BroadcastNotificationAsync(string message);
}
