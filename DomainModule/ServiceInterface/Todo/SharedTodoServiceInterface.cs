using DomainModule.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainModule.ServiceInterface
{
    public interface SharedTodoServiceInterface
    {
        Task Create(SharedTodoCreateDto dto);
        Task<SharedTodoOfTodo> GetSharedTodosListOfTodo(int todoId);
        Task<List<SharedTodoDto>> GetAllTodosSharedToCurrentUser(string currentUserId);
    }
}
