using DomainModule.Dto;
using DomainModule.Exceptions;
using DomainModule.RepositoryInterface;
using DomainModule.ServiceInterface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceModule.Service
{
    public class NotificationService : NotificationServiceInterface
    {
        private readonly NotificationRepositoryInterface _notificationRepo;
        private readonly IUnitOfWork _unitOfWork;
        public NotificationService(NotificationRepositoryInterface notificationRepo,
            IUnitOfWork unitOfWork)
        {
            _notificationRepo = notificationRepo;
            _unitOfWork = unitOfWork;
        }

        public async Task<List<NotificationDto>> GetAllNotifications(string userId)
        {
            try
            {
                var notifications = await _notificationRepo.GetQueryable().Where(a => a.UserId == userId).OrderByDescending(a => a.Id).ToListAsync().ConfigureAwait(false);
                var returnData = new List<NotificationDto>();
                foreach (var notification in notifications)
                {
                    returnData.Add(new NotificationDto
                    {
                        Id = notification.Id,
                        MarkedAsRead = notification.MarkedAsRead,
                        Comment = notification.NotificationMessage
                    });

                }
                return returnData;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task<int> GetNotificationCount(string userId)
        {
            try
            {
                var notifications = await _notificationRepo.GetQueryable().Where(a => a.UserId == userId && !a.MarkedAsRead).CountAsync().ConfigureAwait(false);
                return notifications;

            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task<NotificationsWithCountDto> GetLatestNotifications(string userId)
        {
            try
            {
                var notificationQueryable = _notificationRepo.GetQueryable().Where(a => a.UserId == userId);
                var unReadNotificationCount = await notificationQueryable.Where(a => !a.MarkedAsRead).CountAsync().ConfigureAwait(false);
                var notifications = await notificationQueryable.OrderByDescending(a => a.Id).Take(3).ToListAsync().ConfigureAwait(false);
                var returnData = new NotificationsWithCountDto
                {
                    TotalCount = unReadNotificationCount
                };

                foreach (var notification in notifications)
                {
                    returnData.Notifications.Add(new NotificationDto
                    {
                        Id = notification.Id,
                        MarkedAsRead = notification.MarkedAsRead,
                        Comment = notification.NotificationMessage
                    });

                }
                return returnData;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task<NotificationResponseDto> MarkNotificationAsRead(int todoId,string userId)
        {

            using (var tx = await _unitOfWork.BeginTransaction(System.Data.IsolationLevel.ReadCommitted))
            {
                try
                {
                    var notification = await _notificationRepo.GetById(todoId).ConfigureAwait(false) ?? throw new CustomException("Notification not found");
                    if (notification.MarkedAsRead) return new NotificationResponseDto() { TodoId = notification.TodoId };
                    notification.SetAsRead();
                    _notificationRepo.Update(notification);
                    await _unitOfWork.CompleteAsync().ConfigureAwait(false);
                    await tx.CommitAsync().ConfigureAwait(false);
                    var todoCount = await GetNotificationCount(userId).ConfigureAwait(false);
                    return new NotificationResponseDto { TodoId = notification.TodoId,TodoCount = todoCount };

                }
                catch (Exception)
                {
                    throw;
                }
            }




        }
    }
}
