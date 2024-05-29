using DomainModule.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainModule.ServiceInterface
{
    public interface INotificationService
    {
        Task<int> GetNotificationCount(string userId);
        Task<NotificationsWithCountDto> GetLatestNotifications(string userId);
        Task<List<NotificationDto>> GetAllNotifications(string userId);
        Task<NotificationResponseDto> MarkNotificationAsRead(int todoId,string userId);
    }
}
