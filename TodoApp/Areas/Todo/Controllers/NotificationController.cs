using DomainModule.ServiceInterface;
using Microsoft.AspNetCore.Mvc;
using System.Drawing.Text;
using TodoApp.Extensions;

namespace TodoApp.Areas.Todo.Controllers
{
    [Area("Todo")]
    public class NotificationController : Controller
    {
        private readonly INotificationService _notificationService;
        public NotificationController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }
        public async Task<IActionResult> Index()
        {
            var notifications = await _notificationService.GetAllNotifications(this.GetCurrentUserId());
            return View(notifications);

        }
    }
}
