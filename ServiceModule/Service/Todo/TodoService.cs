using DomainModule.Dto;
using DomainModule.Entity;
using DomainModule.Enums;
using DomainModule.Exceptions;
using DomainModule.RepositoryInterface;
using DomainModule.ServiceInterface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ServiceModule.Service
{
    public class TodoService : TodoServiceInterface
    {
        private readonly TodoRepositoryInterface _todoRepo;
        private readonly UserRepositoryInterface _userRepo;
        private readonly IUnitOfWork _unitOfWork;
        public TodoService(TodoRepositoryInterface todoRepo,
            UserRepositoryInterface userRepo,
            IUnitOfWork unitOfWork)
        {
            _todoRepo = todoRepo;
            _userRepo = userRepo;
            _unitOfWork = unitOfWork;
        }
        public async Task Create(TodoCreateDto dto)
        {
            using (var tx = await _unitOfWork.BeginTransaction(System.Data.IsolationLevel.ReadCommitted))
            {
                try
                {
                    var user = await _userRepo.GetByIdString(dto.UserId).ConfigureAwait(false) ?? throw new UserNotFoundException();
                    var todo = new TodoEntity(dto.Title, dto.PriorityLevel, user, dto.DueDate)
                    {
                        Description = dto.Description
                    };
                    await _todoRepo.InsertAsync(todo).ConfigureAwait(false);
                    await _unitOfWork.CompleteAsync();

                }
                catch (Exception ex)
                {
                    await tx.RollbackAsync().ConfigureAwait(false);
                    throw;
                }
                
            }
        }

        public async Task Delete(int todoId)
        {
            using(var tx =await _unitOfWork.BeginTransaction(System.Data.IsolationLevel.ReadCommitted))
            {
                try
                {
                    var todo = await _todoRepo.GetById(todoId).ConfigureAwait(false) ?? throw new CustomException("Todo Not Found");
                    _todoRepo.Delete(todo);
                    await _unitOfWork.CompleteAsync().ConfigureAwait(false);
                    await tx.CommitAsync().ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    await tx.RollbackAsync().ConfigureAwait(false);
                    throw;
                }
              
            }
        }

        public async Task<List<TodoDto>> GetAllTodosOfUser(string userId)
        {
            try
            {
                var allTodosOfUser = await _todoRepo.GetQueryable().Where(a => a.Status == TodoEntity.StatusActive && a.CreatedBy == userId).OrderByDescending(b => b.PriorityLevel).ThenBy(a => a.DueDate).ToListAsync().ConfigureAwait(false);
                var returnModel = new List<TodoDto>();
                foreach(var  todo in allTodosOfUser)
                {
                    var model = new TodoDto()
                    {
                        Id = todo.Id,
                        PriorityLevel = Enum.GetName(typeof(TodoPriorityEnum),todo.PriorityLevel),
                        Description= todo.Description,
                        DueDate = todo.DueDate,
                        Status = todo.Status,
                        Title = todo.Title,
                        CreatedOn = todo.CreatedOn,
                        CreatedByUser = todo.CreatedByUser.Name,
                    };
                    returnModel.Add(model);
                }
                return returnModel;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task MarkAsComplete(int todoId,string userId)
        {
            using (var tx = await _unitOfWork.BeginTransaction(System.Data.IsolationLevel.ReadCommitted))
            {
                try
                {
                    var todo = await _todoRepo.GetById(todoId).ConfigureAwait(false) ?? throw new CustomException("Todo Not Found");
                    var user = await _userRepo.GetByIdString(userId).ConfigureAwait(false) ?? throw new UserNotFoundException();
                    todo.MarkAsComplete(user);
                    _todoRepo.Update(todo);
                    await _unitOfWork.CompleteAsync().ConfigureAwait(false);
                    await tx.CommitAsync().ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    await tx.RollbackAsync().ConfigureAwait(false);
                    throw;
                }

            }

        }

        public async Task Update(TodoEditDto dto)
        {
            using(var tx = await _unitOfWork.BeginTransaction(System.Data.IsolationLevel.ReadCommitted))
            {
                try
                {
                    var todo = await _todoRepo.GetById(dto.Id).ConfigureAwait(false) ?? throw new CustomException("Todo not found");
                    todo.Update(dto.Title, dto.PriorityLevel, dto.DueDate);
                    todo.Description = dto.Description;
                    _todoRepo.Update(todo);
                    await _unitOfWork.CompleteAsync().ConfigureAwait(false);
                    await tx.CommitAsync().ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                   await  tx.RollbackAsync();
                    throw;
                }
               
            }
        }
    }
}
