﻿using System;
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
            Status = StatusActive;
           
        }

        public void Update(string title,int priorityLevel,DateTime dueDate)
        {
            Title = Title;
            PriorityLevel = priorityLevel;
            DueDate = dueDate;
            ModifiedOn = DateTime.Now;
        }

        public int Id { get; protected set; }
        public string Title { get;protected set; }
        public string Status { get; protected set; }
        public int PriorityLevel { get; protected set; }
        public DateTime CreatedOn { get;protected set; }
        public DateTime? ModifiedOn { get;protected set; }
        public DateTime? CompletedOn { get;protected set; }
        public DateTime DueDate { get;protected set; }
        public string? Description { get; set; }
        public string CreatedBy { get; protected set; }
        public string? CompletedBy { get;protected set; }
        public virtual User CreatedByUser { get;protected set; }
        public virtual User? CompletedByUser { get;protected set; }
        public virtual IList<SharedTodoEntity> SharedTodos { get;protected set; } = new List<SharedTodoEntity>();
        public virtual IList<TodoHistory> SharedTodoHistory { get; protected set; } = new List<TodoHistory>();
        public virtual IList<TodoRemainder> TodoRemainder { get; protected set; } = new List<TodoRemainder>();
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
