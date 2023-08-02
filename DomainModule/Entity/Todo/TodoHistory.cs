using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainModule.Entity
{
    public class TodoHistory
    {
        public const string StatusShared = "Shared";
        public const string StatusCommented = "Commented";
        public const string StatusClosed = "Closed";
        public const string StatusOpened = "Opened";
        protected TodoHistory()
        {
            
        }
        public TodoHistory(TodoEntity todo, User commentedBy, string status, string comment)
        {
            Todo = todo;
            CommentedByUser = commentedBy;
            Status = status;
            Comment = comment;
            CreatedOn = DateTime.Now;
        }
        public int Id { get; protected set; }
        public int TodoId { get; protected set; }
        //commenter on todo or the person who shared todo
        public string CommentedByUserId { get; set; }
        public string Comment { get; set; }
        public DateTime CreatedOn { get; set; }
        public virtual User CommentedByUser { get; set; }
        public virtual TodoEntity Todo { get; set; }
        public string Status { get; set; }
    }


}
