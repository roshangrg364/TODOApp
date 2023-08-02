using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainModule.Dto
{
    public class TodoHistoryDto
    {
        public int Id { get; set; }
        public string? Comment { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CommentedBy { get; set; }
        public string Status { get; set; }
    }

    public class TodoHistoryCreateDto {
        public TodoHistoryCreateDto( string userId,int todoId,string comment)
        {
            UserId = userId;
            TodoId = todoId;
            Comment = comment;
        }
        public string Comment { get; set; }
        public string UserId { get; set; }
        public int TodoId { get; set; }
    }

}
