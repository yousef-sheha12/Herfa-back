using Herfa_back.DTOs.Notification;
using Herfa_back.Models.Enums;

namespace Herfa_back.Interfaces.IService
{
    public interface INotificationService
    {
        Task SendAsync(int userId, string title, string message, NotificationType type, int? referenceId = null, decimal? price = null, string? comment = null);
        Task CreateNotificationAsync(int userId, string title, string message, NotificationType type, int? referenceId = null, decimal? price = null, string? comment = null);
        Task PushToUserAsync(int userId, NotificationDto notification);
        Task<List<NotificationDto>> GetUserNotificationsAsync(int userId);
        Task<bool> MarkAsReadAsync(int notificationId, int userId);
        Task<bool> MarkAllAsReadAsync(int userId);
        Task<int> GetUnreadCountAsync(int userId);
        Task<bool> DeleteAsync(int notificationId, int userId);
        Task<bool> DeleteAllAsync(int userId);
    }
}