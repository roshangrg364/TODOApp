using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainModule.Dto.Todo
{
    public class SharedTodoUserAndLatestCommentDto
    {
        public List<string> SharedTodoUsersList { get; set; } = new List<string>();
        public TodoHistoryDto LatestTodoHistory { get; set; }
    }

    public class TodoCompleteDto
    {
        public List<string> SharedTodoUsersList { get; set; } = new List<string>();
        public TodoDetailsDto TodoDetails { get; set; }
    }

}
