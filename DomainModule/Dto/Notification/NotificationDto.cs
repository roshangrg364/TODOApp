using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainModule.Dto
{
    public class NotificationDto
    {
        public int Id { get; set; }
        public string Comment { get; set; }
        public bool MarkedAsRead { get; set; }
    }
    public class NotificationsWithCountDto
    {
        public int TotalCount { get; set; }
        public List<NotificationDto> Notifications { get; set; } = new List<NotificationDto>();
    }

    public class NotificationResponseDto
    {
        public int? TodoId { get; set; }
        public int? TodoCount{ get; set; }
    }


}
