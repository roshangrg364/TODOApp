using DomainModule.Dto;
using DomainModule.Dto.Todo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainModule.ServiceInterface
{
    public interface TodoServiceInterface
    {
        Task<int> Create(TodoCreateDto dto);
        Task Update(TodoEditDto dto);
        Task Delete(int todoId);
        Task<List<TodoDto>> GetAllTodosOfUser(TodoFilterDto filter);
        Task<TodoCompleteDto> MarkAsComplete(TodoHistoryCreateDto dto);
        Task<TodoResponseDto> GetById(int todoId);
        Task<TodoDetailsDto> GetTodoDetails(int todoId,string userId);
        Task<SharedTodoUserAndLatestCommentDto> CommentOnTodo(TodoHistoryCreateDto dto);
        Task SetRemainder(string userId,int todoId,DateTime remainderOn);
        Task UnsetRemainder(string userId,int todoId);
        Task<List<string>> GetTodoRemainder(string userId);
    }
}
