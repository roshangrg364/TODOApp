using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainModule.Entity
{
    public class SharedTodoEntity
    {
        protected SharedTodoEntity()
        {
            
        }
        public SharedTodoEntity(TodoEntity todo,User user)
        {
            Todo = todo;
            User = user;
        }
        public int Id { get;protected set; }
        public int TodoId { get; set; }
        public virtual TodoEntity Todo { get;protected set; }
        public string UserId { get; set; }
        public virtual User User { get; set; }
        public string? Description { get; set; }
    }
}
