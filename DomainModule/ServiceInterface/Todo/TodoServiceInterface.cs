﻿using DomainModule.Dto;
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
        Task<TodoDetailsDto> MarkAsComplete(TodoHistoryCreateDto dto);
        Task<TodoResponseDto> GetById(int todoId);
        Task<TodoDetailsDto> GetTodoDetails(int todoId);
        Task<TodoDetailsDto> CommentOnTodo(TodoHistoryCreateDto dto);

    }
}
