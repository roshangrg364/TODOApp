using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainModule.Entity
{
    public class TodoEntity
    {
        public const string StatusActive = "Active";
        public const string StatusCompleted = "Completed";
        protected TodoEntity()
        {
            
        }
        public TodoEntity(string title,int priorityLevel,User createdBy,DateTime dueDate)
        {
            Title = title;
            PriorityLevel = priorityLevel;
            CreatedOn = DateTime.Now;
            CreatedByUser = createdBy;
            DueDate = dueDate;
        }

        public void Update(string title,int priorityLevel,DateTime dueDate)
        {
            Title = Title;
            PriorityLevel = priorityLevel;
            DueDate = dueDate;
        }

        public int Id { get; protected set; }
        public string Title { get;protected set; }
        public string Status { get; protected set; }
        public int PriorityLevel { get; protected set; }
        public DateTime CreatedOn { get;protected set; }
        public DateTime? CompletedOn { get;protected set; }
        public DateTime DueDate { get; set; }
        public string? Description { get; set; }
        public string CreatedBy { get; protected set; }
        public string? CompletedBy { get; set; }
        public virtual User CreatedByUser { get; set; }
        public virtual User? CompletedByUser { get; set; }
        public virtual IList<SharedTodoEntity> SharedTodos { get; set; } = new List<SharedTodoEntity>();
        public bool IsActive => Status == StatusActive;
        public bool IsCompleted => Status == StatusCompleted;
        public void MarkAsComplete(User completedBy)
        {
            CompletedByUser = completedBy;
            CompletedOn = DateTime.Now;
            Status = StatusCompleted;
        }

    }
}
