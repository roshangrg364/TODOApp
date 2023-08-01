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
        public string? Description { get; set; }
        public DateTime TodoDueDate { get; set; }
        public string SharedBy { get; set; }
        public string SharedTo { get; set; }
    }

    public class SharedTodoCreateDto
    {
        public SharedTodoCreateDto(string email,int todoId,string description)
        {
            Email = email;
            TodoId = todoId;
            Description = description;
        }
        public string Email { get; set; }
        public int TodoId { get; set; }
        public string Description { get; set; }

    }

    public class SharedTodoOfTodo
    {
        public string Title { get; set; }
        public string CreatedBy { get; set; }
        public string? Description { get; set; }
        public List<SharedTodoDto> SharedTodos { get; set; } = new List<SharedTodoDto>();
    }

}
