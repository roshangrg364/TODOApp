using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainModule.Dto
{
    public class SharedTodoDto
    {
        public int Id { get; set; }
        public int TodoId { get; set; }
        public string TodoTitle { get; set; }
        public string PriorityLevel  { get; set; }
        public string? Description { get; set; }
        public DateTime TodoDueDate { get; set; }
        public DateTime CreatedOn { get; set; }
        public string SharedBy { get; set; }
        public string SharedTo { get; set; }
        public string Status { get; set; }
    }

    public class SharedTodoCreateDto
    {
        public SharedTodoCreateDto(string email,int todoId, string description, string currentUserId)
        {
            Email = email;
            TodoId = todoId;
            Description = description;
            CurrentUserId = currentUserId;  

        }
        public string Email { get; set; }
        public int TodoId { get; set; }
        public string Description { get; set; }
        public string CurrentUserId { get; set; }

    }

    public class SharedTodoFilterDto
    {
        public string? Title { get; set; }
        public string? Status { get; set; }
        public string CurrentUserId { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public int? PriorityLevel { get; set; }
    }

}
