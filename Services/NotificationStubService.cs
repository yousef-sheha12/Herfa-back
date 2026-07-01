using Herfa_back.Interfaces.IService;

namespace Herfa_back.Services
{
    // Temporary stub — Person 5 will replace this with real implementation
    public class NotificationStubService : INotificationService
    {
        public Task SendAsync(int userId, string title, string message, string type)
        {
            // Person 5 will implement real notification logic here
            return Task.CompletedTask;
        }
    }
}