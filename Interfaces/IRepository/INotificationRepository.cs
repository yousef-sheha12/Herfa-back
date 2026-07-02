using Herfa_back.Models;

namespace Herfa_back.Interfaces.IRepository
{
    public interface INotificationRepository
    {
        Task<List<Notification>> GetByUserIdAsync(int userId);
        Task<Notification?> GetByIdAsync(int id);
        Task AddAsync(Notification notification);
        Task SaveChangesAsync();
        Task MarkAsReadAsync(int notificationId, int userId);
        Task MarkAllAsReadAsync(int userId);
        Task<int> GetUnreadCountAsync(int userId);
        Task<bool> DeleteAsync(int notificationId, int userId);
        Task<bool> DeleteAllAsync(int userId);
    }
}
