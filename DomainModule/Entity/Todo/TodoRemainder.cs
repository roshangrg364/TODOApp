using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainModule.Entity
{
    public class TodoRemainder
    {
        protected TodoRemainder()
        {
            
        }
        public TodoRemainder(TodoEntity todo,User setBy,DateTime remainderOn)
        {
            Todo = todo;
            SetBy = setBy;
            RemainderOn = remainderOn;
            IsActive = true;
            CreatedOn = DateTime.Now;
        }

        public void MarkAsComplete()
        {
            IsActive = false;
        }
        public int Id { get;protected set; }
        public int TodoId { get;protected set; }
        public virtual TodoEntity Todo { get;protected set; }
        public string SetById { get; protected set; }
        public virtual User SetBy { get;protected set; }
        public DateTime RemainderOn { get; set; }
        public bool IsActive { get;protected set; }
        public DateTime CreatedOn { get; set; }
    }
}
