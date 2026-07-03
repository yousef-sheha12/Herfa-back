using Herfa_back.DTOs.Notification;
using Herfa_back.Hubs;
using Herfa_back.Interfaces.IRepository;
using Herfa_back.Interfaces.IService;
using Herfa_back.Models;
using Herfa_back.Models.Enums;
using Microsoft.AspNetCore.SignalR;

namespace Herfa_back.Services
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _repository;
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly ILogger<NotificationService> _logger;

        public NotificationService(
            INotificationRepository repository,
            IHubContext<NotificationHub> hubContext,
            ILogger<NotificationService> logger)
        {
            _repository = repository;
            _hubContext = hubContext;
            _logger = logger;
        }

        public async Task CreateNotificationAsync(int userId, string title, string message, NotificationType type, int? referenceId = null, decimal? price = null, string? comment = null)
        {
            var notification = new Notification
            {
                UserId = userId,
                Title = title,
                Message = message,
                Type = type,
                IsRead = false,
                ReferenceId = referenceId,
                Price = price,
                Comment = comment,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _repository.AddAsync(notification);
        }

        public async Task<List<NotificationDto>> GetUserNotificationsAsync(int userId)
        {
            var notifications = await _repository.GetByUserIdAsync(userId);
            return notifications.Select(n => new NotificationDto
            {
                Id = n.Id,
                Title = n.Title,
                Message = n.Message,
                Type = n.Type.ToString(),
                IsRead = n.IsRead,
                ReferenceId = n.ReferenceId,
                Price = n.Price,
                Comment = n.Comment,
                CreatedAt = n.CreatedAt
            }).ToList();
        }

        public async Task<bool> MarkAllAsReadAsync(int userId)
        {
            await _repository.MarkAllAsReadAsync(userId);
            return true;
        }

        public async Task<bool> MarkAsReadAsync(int notificationId, int userId)
        {
            var notification = await _repository.GetByIdAsync(notificationId);
            if (notification == null || notification.UserId != userId) return false;

            await _repository.MarkAsReadAsync(notificationId, userId);
            return true;
        }

        public async Task PushToUserAsync(int userId, NotificationDto notification)
        {
            try
            {
                await _hubContext.Clients.User(userId.ToString())
                    .SendAsync("ReceiveNotification", notification);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SignalR push failed for user {UserId}", userId);
            }
        }

        public async Task SendAsync(int userId, string title, string message, NotificationType type, int? referenceId = null, decimal? price = null, string? comment = null)
        {
            var notification = new Notification
            {
                UserId = userId,
                Title = title,
                Message = message,
                Type = type,
                IsRead = false,
                ReferenceId = referenceId,
                Price = price,
                Comment = comment,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _repository.AddAsync(notification);
            await _repository.SaveChangesAsync();

            var dto = new NotificationDto
            {
                Id = notification.Id,
                Title = notification.Title,
                Message = notification.Message,
                Type = notification.Type.ToString(),
                IsRead = notification.IsRead,
                ReferenceId = notification.ReferenceId,
                Price = notification.Price,
                Comment = notification.Comment,
                CreatedAt = notification.CreatedAt
            };

            await PushToUserAsync(userId, dto);
        }

        public async Task<int> GetUnreadCountAsync(int userId)
        {
            return await _repository.GetUnreadCountAsync(userId);
        }

        public async Task<bool> DeleteAsync(int notificationId, int userId)
        {
            return await _repository.DeleteAsync(notificationId, userId);
        }

        public async Task<bool> DeleteAllAsync(int userId)
        {
            return await _repository.DeleteAllAsync(userId);
        }
    }
}
