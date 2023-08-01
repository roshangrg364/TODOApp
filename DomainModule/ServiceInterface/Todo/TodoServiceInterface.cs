using DomainModule.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainModule.ServiceInterface
{
    public interface TodoServiceInterface
    {
        Task Create(TodoCreateDto dto);
        Task Update(TodoEditDto dto);
        Task Delete(int todoId);
        Task<List<TodoDto>> GetAllTodosOfUser(string userId);
        Task MarkAsComplete(int todoId,string userId);
    }
}
