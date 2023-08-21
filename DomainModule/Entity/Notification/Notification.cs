using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainModule.Entity
{
    public class Notification
    {

        protected Notification()
        {
            
        }
        public Notification(User user, string notificationMessage, TodoEntity? todo)
        {
            User = user;
            NotificationMessage = notificationMessage;
            Todo = todo;
            CreatedOn = DateTime.Now;
            MarkedAsRead = false;
        }
        public void SetAsRead()
        {
            MarkedAsRead = true;
        }
         public int Id { get; protected set; }
        public string UserId { get;protected set; }
        public string NotificationMessage { get; protected set; }
        public bool MarkedAsRead { get; protected set; }
        public int? TodoId { get; protected set; }
        public DateTime CreatedOn { get; protected set; }
        public virtual User User { get; protected set; }
        public virtual TodoEntity? Todo { get; protected set; }
    }

}
