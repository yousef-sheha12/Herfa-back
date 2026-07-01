namespace Herfa_back.Interfaces.IService
{
    public interface INotificationService
    {
        Task SendAsync(int userId, string title, string message, string type);
    }
}