using Herfa_back.Interfaces.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Herfa_back.Controllers
{
    [ApiController]
    [Route("api/notifications")]
    [Authorize]
    public class NotificationsController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public NotificationsController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [HttpGet]
        public async Task<IActionResult> GetMyNotifications()
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdStr, out int userId))
            {
                return Unauthorized();
            }

            var notifications = await _notificationService.GetUserNotificationsAsync(userId);
            return Ok(notifications);
        }

        [HttpPatch("{id}/read")]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdStr, out int userId))
            {
                return Unauthorized();
            }

            var success = await _notificationService.MarkAsReadAsync(id, userId);
            if (!success)
            {
                return NotFound("Notification not found or access denied.");
            }

            return Ok(new { message = "Notification marked as read." });
        }

        [HttpPatch("read-all")]
        public async Task<IActionResult> MarkAllAsRead()
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdStr, out int userId))
            {
                return Unauthorized();
            }

            await _notificationService.MarkAllAsReadAsync(userId);
            return Ok(new { message = "All notifications marked as read." });
        }

        [HttpGet("unread")]
        public async Task<IActionResult> GetUnreadCount()
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdStr, out int userId))
            {
                return Unauthorized();
            }

            var count = await _notificationService.GetUnreadCountAsync(userId);
            return Ok(new { unreadCount = count });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNotification(int id)
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdStr, out int userId))
            {
                return Unauthorized();
            }

            var success = await _notificationService.DeleteAsync(id, userId);
            if (!success)
            {
                return NotFound("Notification not found or access denied.");
            }

            return Ok(new { message = "Notification deleted." });
        }

        [HttpDelete("clear-all")]
        public async Task<IActionResult> ClearAllNotifications()
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdStr, out int userId))
            {
                return Unauthorized();
            }

            var success = await _notificationService.DeleteAllAsync(userId);
            if (!success)
            {
                return Ok(new { message = "No notifications to clear." });
            }

            return Ok(new { message = "All notifications cleared." });
        }
    }
}
