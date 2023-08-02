using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainModule.Dto
{
    public class DashboardDto
    {
        public long TotalTodoCount { get; set; }
        public long TotalUser { get; set; }
        public long TotalSharedTodoCount { get; set; }
        public long TotalActiveTodoCount { get; set; }
        public long TotalCompletedTodo { get; set; }
        public List<int> TotalTodosEachMonth { get; set; } = new List<int>();
       
    }

}
