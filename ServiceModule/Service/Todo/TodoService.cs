using DomainModule.Dto;
using DomainModule.Entity;
using DomainModule.Enums;
using DomainModule.Exceptions;
using DomainModule.RepositoryInterface;
using DomainModule.ServiceInterface;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
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
        private readonly TodoHistoryRepositoryInterface _todoHistoryRepo;
        private readonly SharedTodoRepositoryInterface _sharedTodoRepo;
        private readonly TodoRemainderRepositoryInterface _todoRemainderRepo;
        public TodoService(TodoRepositoryInterface todoRepo,
            UserRepositoryInterface userRepo,
            IUnitOfWork unitOfWork,
            TodoHistoryRepositoryInterface todoHistoryRepo,
            SharedTodoRepositoryInterface sharedTodoRepo,
            TodoRemainderRepositoryInterface todoRemainderRepo)
        {
            _todoRepo = todoRepo;
            _userRepo = userRepo;
            _unitOfWork = unitOfWork;
            _todoHistoryRepo = todoHistoryRepo;
            _sharedTodoRepo = sharedTodoRepo;
            _todoRemainderRepo = todoRemainderRepo;
        }

        public async Task<TodoDetailsDto> GetTodoDetails(int todoId, string userId)
        {
            try
            {
                var todo = await _todoRepo.GetQueryable().Include(a => a.SharedTodoHistory).Include(a => a.SharedTodos).Include(a => a.TodoRemainder).Include(a => a.CreatedByUser).Include(a => a.CompletedByUser).Where(a => a.Id == todoId).FirstOrDefaultAsync().ConfigureAwait(false) ?? throw new CustomException("Todo not found");
                var todoDetailsDto = new TodoDetailsDto()
                {
                    Id = todo.Id,
                    Title = todo.Title,
                    CreatedBy = todo.CreatedByUser.Name,
                    CreatedByUserId = todo.CreatedBy,
                    Description = todo.Description,
                    Status = todo.Status,
                    PriorityLevel = Enum.GetName(typeof(TodoPriorityEnum), todo.PriorityLevel),
                    DueDate = todo.DueDate,
                    CreatedOn = todo.CreatedOn,
                    ModifiedOn = todo.ModifiedOn,
                    CompletedBy = todo.CompletedByUser != null ? todo.CompletedByUser.Name : string.Empty,
                    CompletedOn = todo.CompletedOn,
                    IsEligible = todo.CreatedBy == userId || todo.SharedTodos.Any(a => a.UserId == userId),
                    HasSetRemainder = todo.TodoRemainder.Any(a => a.IsActive && a.SetById == userId)
                };

                foreach (var history in todo.SharedTodoHistory.OrderBy(a => a.CreatedOn))
                {
                    var sharedTodoHistoryModel = new TodoHistoryDto
                    {
                        CommentedBy = history.CommentedByUser.Name,
                        Comment = history.Comment,
                        CreatedOn = history.CreatedOn,
                        Status = history.Status
                    };
                    todoDetailsDto.SharedTodoHistory.Add(sharedTodoHistoryModel);
                }
                return todoDetailsDto;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task<int> Create(TodoCreateDto dto)
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

                    await _unitOfWork.CompleteAsync().ConfigureAwait(false);
                    var todoHistory = AddSharedTodoHistory(todo, user, $"Todo Created By {user.Name}", TodoHistory.StatusOpened);
                    await _todoHistoryRepo.InsertAsync(todoHistory).ConfigureAwait(false);
                    await _unitOfWork.CompleteAsync().ConfigureAwait(false);
                    await tx.CommitAsync();
                    return todo.Id;
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
            using (var tx = await _unitOfWork.BeginTransaction(System.Data.IsolationLevel.ReadCommitted))
            {
                try
                {
                    var todo = await _todoRepo.GetById(todoId).ConfigureAwait(false) ?? throw new CustomException("Todo Not Found");
                    ValidateTodo(todo);
                    var todoHistory = todo.SharedTodoHistory.ToList();
                    _todoHistoryRepo.DeleteRange(todoHistory);
                    _sharedTodoRepo.DeleteRange(todo.SharedTodos.ToList());
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

        public async Task<List<TodoDto>> GetAllTodosOfUser(TodoFilterDto filter)
        {
            try
            {
                IQueryable<TodoEntity> allTodosOfUserQueryable = await FilterTodos(filter).ConfigureAwait(false);
                var allTodosOfuser = await allTodosOfUserQueryable.Where(a => a.DueDate.Date >= filter.FromDate.Date && a.DueDate.Date <= filter.ToDate.Date).OrderBy(a => a.Status).ThenByDescending(b => b.PriorityLevel).ThenBy(a => a.DueDate).ToListAsync().ConfigureAwait(false);
                var returnModel = new List<TodoDto>();
                foreach (var todo in allTodosOfuser)
                {
                    var model = new TodoDto()
                    {
                        Id = todo.Id,
                        PriorityLevel = Enum.GetName(typeof(TodoPriorityEnum), todo.PriorityLevel),
                        Description = todo.Description,
                        DueDate = todo.DueDate,
                        Status = todo.Status,
                        Title = todo.Title,
                        CreatedOn = todo.CreatedOn,
                        CreatedByUser = todo.CreatedByUser.Name,
                        CreatedBy = todo.CreatedBy
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

        private async Task<IQueryable<TodoEntity>> FilterTodos(TodoFilterDto filter)
        {
            var allTodosOfUserQueryable = _todoRepo.GetQueryable();
            if (!string.IsNullOrEmpty(filter.Title))
            {
                allTodosOfUserQueryable = allTodosOfUserQueryable.Where(a => a.Title.ToLower().Contains(filter.Title));
            };
            if (filter.PriorityLevel != null)
            {
                allTodosOfUserQueryable = allTodosOfUserQueryable.Where(a => a.PriorityLevel == filter.PriorityLevel);
            }

            if (!string.IsNullOrEmpty(filter.Status))
            {
                allTodosOfUserQueryable = allTodosOfUserQueryable.Where(a => a.Status == filter.Status);
            }
            var currrentUser = await _userRepo.GetByIdString(filter.CurrentUserId).ConfigureAwait(false) ?? throw new UserNotFoundException();
            if (!currrentUser.IsSuperAdmin)
            {
                allTodosOfUserQueryable = allTodosOfUserQueryable.Where(a => a.CreatedBy == filter.CurrentUserId);

            };

            return allTodosOfUserQueryable;
        }

        public async Task<TodoResponseDto> GetById(int todoId)
        {
            try
            {
                var todo = await _todoRepo.GetById(todoId).ConfigureAwait(false) ?? throw new CustomException("Todo not found");
                return new TodoResponseDto
                {
                    Id = todo.Id,
                    Description = todo.Description,
                    Title = todo.Title,
                    DueDate = todo.DueDate,
                    PriorityLevel = todo.PriorityLevel,
                    CreatedBy = todo.CreatedByUser.Name,
                    CreatedOn = todo.CreatedOn.ToString("yyyy-MM-dd")
                };

            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task<TodoDetailsDto> MarkAsComplete(TodoHistoryCreateDto dto)
        {
            using (var tx = await _unitOfWork.BeginTransaction(System.Data.IsolationLevel.ReadCommitted))
            {
                try
                {
                    var todo = await _todoRepo.GetById(dto.TodoId).ConfigureAwait(false) ?? throw new CustomException("Todo Not Found");
                    ValidateTodo(todo);
                    var user = await _userRepo.GetByIdString(dto.UserId).ConfigureAwait(false) ?? throw new UserNotFoundException();
                    todo.MarkAsComplete(user);
                    var todoHistory = AddSharedTodoHistory(todo, user, dto.Comment, TodoHistory.StatusClosed);
                    _todoRepo.Update(todo);
                    await _todoHistoryRepo.InsertAsync(todoHistory).ConfigureAwait(false);
                    await _unitOfWork.CompleteAsync().ConfigureAwait(false);
                    await tx.CommitAsync().ConfigureAwait(false);
                    var todoDetails = await GetTodoDetails(dto.TodoId, dto.UserId).ConfigureAwait(false);
                    return todoDetails;
                }
                catch (Exception ex)
                {
                    await tx.RollbackAsync().ConfigureAwait(false);
                    throw;
                }

            }

        }

        public async Task<TodoDetailsDto> CommentOnTodo(TodoHistoryCreateDto dto)
        {
            using (var tx = await _unitOfWork.BeginTransaction(System.Data.IsolationLevel.ReadCommitted))
            {
                try
                {
                    var todo = await _todoRepo.GetById(dto.TodoId).ConfigureAwait(false) ?? throw new CustomException("todo not found");
                    if (todo.IsCompleted) throw new CustomException("Todo already completed");
                    var user = await _userRepo.GetByIdString(dto.UserId).ConfigureAwait(false) ?? throw new UserNotFoundException();
                    if (string.IsNullOrEmpty(dto.Comment)) throw new CustomException("Comment is required");
                    var sharedTodoHistory = AddSharedTodoHistory(todo, user, dto.Comment, TodoHistory.StatusCommented);
                    await _todoHistoryRepo.InsertAsync(sharedTodoHistory).ConfigureAwait(false);
                    await _unitOfWork.CompleteAsync().ConfigureAwait(false);
                    await tx.CommitAsync().ConfigureAwait(false);
                    var todoDetails = await GetTodoDetails(dto.TodoId, dto.UserId).ConfigureAwait(false);
                    return todoDetails;
                }

                catch (Exception)
                {
                    await tx.RollbackAsync().ConfigureAwait(false);
                    throw;
                }
            }
        }

        private static void ValidateTodo(TodoEntity todo)
        {
            if (todo.IsCompleted) throw new CustomException("Todo Already Completed");
        }
        private TodoHistory AddSharedTodoHistory(TodoEntity todo, User user, string comment, string status)
        {
            var sharedTodoHistory = new TodoHistory(todo, user, status, comment);
            return sharedTodoHistory;
        }


        public async Task Update(TodoEditDto dto)
        {
            using (var tx = await _unitOfWork.BeginTransaction(System.Data.IsolationLevel.ReadCommitted))
            {
                try
                {
                    var todo = await _todoRepo.GetById(dto.Id).ConfigureAwait(false) ?? throw new CustomException("Todo not found");
                    ValidateTodo(todo);
                    todo.Update(dto.Title, dto.PriorityLevel, dto.DueDate);
                    todo.Description = dto.Description;
                    _todoRepo.Update(todo);
                    await _unitOfWork.CompleteAsync().ConfigureAwait(false);
                    await tx.CommitAsync().ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    await tx.RollbackAsync();
                    throw;
                }

            }
        }

        public async Task<TodoDetailsDto> SetRemainder(string userId, int todoId, DateTime remainderOn)
        {
            using (var tx = await _unitOfWork.BeginTransaction(System.Data.IsolationLevel.ReadCommitted))
            {
                try
                {
                    var todo = await _todoRepo.GetById(todoId).ConfigureAwait(false) ?? throw new CustomException("Todo Not Found");
                    var user = await _userRepo.GetByIdString(userId).ConfigureAwait(false) ?? throw new UserNotFoundException();
                    var existingTodoRemainder = await _todoRemainderRepo.GetQueryable().Where(a => a.TodoId == todoId && a.SetById == userId && a.IsActive).FirstOrDefaultAsync().ConfigureAwait(false);
                    if (existingTodoRemainder != null)
                    {
                        existingTodoRemainder.RemainderOn = remainderOn;
                    }
                    else
                    {
                        var todoRemainder = new TodoRemainder(todo, user, remainderOn);
                        await _todoRemainderRepo.InsertAsync(todoRemainder).ConfigureAwait(false);

                    }
                    await _unitOfWork.CompleteAsync().ConfigureAwait(false);
                    await tx.CommitAsync().ConfigureAwait(false);
                    var response = await GetTodoDetails(todoId, userId).ConfigureAwait(false);
                    return response;

                }
                catch (Exception ex)
                {
                    await tx.RollbackAsync().ConfigureAwait(false);
                    throw;
                }
            }
        }

        public async Task<TodoDetailsDto> UnsetRemainder(string userId, int todoId)
        {
            using (var tx = await _unitOfWork.BeginTransaction(System.Data.IsolationLevel.ReadCommitted))
            {
                try
                {
                    var todo = await _todoRepo.GetById(todoId).ConfigureAwait(false) ?? throw new CustomException("Todo Not Found");
                    if (todo.IsCompleted) throw new CustomException("Todo already completed");
                    var todoRemainderOfUser = await _todoRemainderRepo.GetQueryable().Where(a => a.TodoId == todoId && a.SetById == userId && a.IsActive).FirstOrDefaultAsync().ConfigureAwait(false);
                    if (todoRemainderOfUser != null)
                    {
                        _todoRemainderRepo.Delete(todoRemainderOfUser);
                        await _unitOfWork.CompleteAsync().ConfigureAwait(false);
                    }
                    await tx.CommitAsync().ConfigureAwait(false);
                    var response = await GetTodoDetails(todoId, userId).ConfigureAwait(false);
                    return response;

                }
                catch (Exception ex)
                {
                    await tx.RollbackAsync().ConfigureAwait(false);
                    throw;
                }
            }

        }

        public async Task<List<string>> GetTodoRemainder(string userId)
        {
            using (var tx = await _unitOfWork.BeginTransaction(System.Data.IsolationLevel.ReadCommitted))
            {
                try
                {
                    if (string.IsNullOrEmpty(userId)) return new List<string>();
                    var todoRemainders = await _todoRemainderRepo.GetQueryable().Where(a => a.IsActive && a.SetById == userId && a.RemainderOn <= DateTime.Now).ToListAsync().ConfigureAwait(false);
                    var messages = new List<string>();
                    foreach (var todo in todoRemainders)
                    {
                        todo.MarkAsComplete();
                        if (!todo.Todo.IsCompleted)
                        {
                            var message = $"Reminder for Todo ({todo.Todo.Title}). Due date : {todo.Todo.DueDate.ToString("yyyy-MM-dd")}";
                            messages.Add(message);
                        }
                    }
                    await _unitOfWork.CompleteAsync().ConfigureAwait(false);
                    await tx.CommitAsync().ConfigureAwait(false);
                    return messages;
                }
                catch (Exception)
                {
                    await tx.RollbackAsync().ConfigureAwait(false);
                    return new List<string>();
                }
            }
        }
    }
}
