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
        Task<List<SharedTodoDto>> GetAllTodosSharedToCurrentUser(SharedTodoFilterDto dto);
       
    }
}
