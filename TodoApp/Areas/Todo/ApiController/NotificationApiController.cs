using DomainModule.ServiceInterface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TodoApp.Extensions;
using TodoApp.Models;
using TodoApp.ViewModel;

namespace TodoApp.Areas.Todo.ApiController
{
    [Route("api/notifications")]
    [ApiController]
    public class NotificationApiController : Controller
    {
        private readonly INotificationService _notificationService;
        public NotificationApiController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [HttpGet("notification-count")]
        public async Task<IActionResult> GetNotificationsCount()
        {
            try
            {
                var notificationCount = await _notificationService.GetNotificationCount(this.GetCurrentUserId()).ConfigureAwait(false);
                return new JsonResult(new ResponseModel { IsSuccess = true, Status = StatusType.success.ToString(), Data = notificationCount });
            }
            catch (Exception ex)
            {
                return new JsonResult(new ResponseModel { IsSuccess = false, Status = StatusType.error.ToString(), Message = ex.Message });
            }
        }

        [HttpGet("notification-view")]
        public async Task<IActionResult> GetNotificationView()
        {
            try
            {
                var latestNotifications = await _notificationService.GetLatestNotifications(this.GetCurrentUserId()).ConfigureAwait(false);
                var notificationView = this.RenderViewAsync("~/Areas/Todo/Views/Notification/_LatestNotificationView.cshtml", latestNotifications, true).GetAwaiter().GetResult();
                return new JsonResult(new ResponseModel { IsSuccess = true, Status = StatusType.success.ToString(), Data = notificationView });
            }
            catch (Exception ex)
            {
                return new JsonResult(new ResponseModel { IsSuccess = false, Status = StatusType.error.ToString(), Message = ex.Message });
            }
        }

        [HttpPut("MarkNotificationAsRead/{id:int}")]
        public async Task<IActionResult> ReadNotification(int id)
        {
            try
            {
                var result = await _notificationService.MarkNotificationAsRead(id,this.GetCurrentUserId()).ConfigureAwait(false);
                return new JsonResult(new ResponseModel { IsSuccess = true, Status = StatusType.success.ToString(), Data = result });
            }
            catch (Exception ex)
            {
                return new JsonResult(new ResponseModel { IsSuccess = false, Status = StatusType.error.ToString(), Message = ex.Message });
            }
        }
    }
}
