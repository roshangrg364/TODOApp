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
using System.Text;
using System.Threading.Tasks;

namespace ServiceModule.Service
{
    public class SharedTodoService : SharedTodoServiceInterface
    {
        private readonly SharedTodoRepositoryInterface _sharedTodoRepo;
        private readonly TodoHistoryRepositoryInterface _todoHistoryRepo;
        private readonly TodoRepositoryInterface _todoRepo;
        private readonly UserRepositoryInterface _userRepo;
        private readonly IUnitOfWork _unitOfWork;
        public SharedTodoService(SharedTodoRepositoryInterface sharedTodoRepo,
            TodoRepositoryInterface todoRepo,
            IUnitOfWork unitOfWork,
            UserRepositoryInterface userRepo,
            TodoHistoryRepositoryInterface todoHistoryRepo)
        {
            _sharedTodoRepo = sharedTodoRepo;
            _todoRepo = todoRepo;
            _unitOfWork = unitOfWork;
            _userRepo = userRepo;
            _todoHistoryRepo = todoHistoryRepo;
        }
        public async Task Create(SharedTodoCreateDto dto)
        {
            using (var tx = await _unitOfWork.BeginTransaction(System.Data.IsolationLevel.ReadCommitted))
            {
                try
                {
                    var todo = await _todoRepo.GetById(dto.TodoId).ConfigureAwait(false) ?? throw new CustomException("Todo does not exists.");
                    User user = await ValidateTodo(dto, todo).ConfigureAwait(false);
                    var sharedTodoEntity = new SharedTodoEntity(todo, user);
                    var comment = $"Todo Shared By {todo.CreatedByUser.Name} to {user.Name} <br/> Comment: {dto.Description}";
                    var sharedTodoHistory = AddSharedTodoHistory(todo, todo.CreatedByUser, comment, TodoHistory.StatusShared);
                    await _sharedTodoRepo.InsertAsync(sharedTodoEntity).ConfigureAwait(false);
                    await _todoHistoryRepo.InsertAsync(sharedTodoHistory).ConfigureAwait(false);
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

        private TodoHistory AddSharedTodoHistory(TodoEntity todo, User user, string comment, string status)
        {
            var sharedTodoHistory = new TodoHistory(todo, user, status, comment);
            return sharedTodoHistory;
        }

        private async Task<User> ValidateTodo(SharedTodoCreateDto dto, TodoEntity todo)
        {
            if (todo.IsCompleted) throw new CustomException("Todo already completed");
            var user = await _userRepo.GetQueryable().Where(a => a.Email == dto.Email).FirstOrDefaultAsync().ConfigureAwait(false) ?? throw new CustomException("User not found");
            var isTodoAlreadyShared = await _sharedTodoRepo.GetQueryable().Where(a => a.UserId == user.Id && a.TodoId == dto.TodoId).FirstOrDefaultAsync().ConfigureAwait(false);
            if (isTodoAlreadyShared != null) throw new CustomException("Todo already shared to the user");
            if (dto.CurrentUserId == user.Id) throw new CustomException("Todo is being shared to the Creator Of todo.");
            return user;
        }

        public async Task<List<SharedTodoDto>> GetAllTodosSharedToCurrentUser(SharedTodoFilterDto filter)
        {
            try
            {
                var sharedTodoQueryAble = await FilterTodos(filter).ConfigureAwait(false);
                var sharedTodosOfUser = await sharedTodoQueryAble.ToListAsync().ConfigureAwait(false);
                var returnModel = new List<SharedTodoDto>();
                foreach (var sharedTodo in sharedTodosOfUser)
                {
                    var model = new SharedTodoDto
                    {
                        Id = sharedTodo.Id,
                        TodoId = sharedTodo.TodoId,
                        TodoTitle = sharedTodo.Todo.Title,
                        Description = sharedTodo.Todo.Description,
                        TodoDueDate = sharedTodo.Todo.DueDate,
                        SharedBy = sharedTodo.Todo.CreatedByUser.Name,
                        SharedTo = sharedTodo.User.Name,
                        PriorityLevel = Enum.GetName(typeof(TodoPriorityEnum), sharedTodo.Todo.PriorityLevel),
                        Status = sharedTodo.Todo.Status
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


        private async Task<IQueryable<SharedTodoEntity>> FilterTodos(SharedTodoFilterDto filter)
        {
            IQueryable<SharedTodoEntity> allTodosOfUserQueryable = _sharedTodoRepo.GetQueryable().Include(a => a.User).Include(a => a.Todo).ThenInclude(a => a.CreatedByUser);
            if (!string.IsNullOrEmpty(filter.Title))
            {
                allTodosOfUserQueryable = allTodosOfUserQueryable.Where(a => a.Todo.Title.Contains(filter.Title));
            };
            if (filter.PriorityLevel != null)
            {
                allTodosOfUserQueryable = allTodosOfUserQueryable.Where(a => a.Todo.PriorityLevel == filter.PriorityLevel);
            }

            if (!string.IsNullOrEmpty(filter.Status))
            {
                allTodosOfUserQueryable = allTodosOfUserQueryable.Where(a => a.Todo.Status == filter.Status);
            }

            var currrentUser = await _userRepo.GetByIdString(filter.CurrentUserId).ConfigureAwait(false) ?? throw new UserNotFoundException();
            if (!currrentUser.IsSuperAdmin)
            {
                allTodosOfUserQueryable = allTodosOfUserQueryable.Where(a => a.UserId == filter.CurrentUserId);

            };
            allTodosOfUserQueryable = allTodosOfUserQueryable.Where(a => a.Todo.DueDate.Date >= filter.FromDate.Date && a.Todo.DueDate.Date <= filter.ToDate.Date).OrderByDescending(b => b.Todo.PriorityLevel).ThenBy(a => a.Todo.DueDate);
            return allTodosOfUserQueryable;
        }

    }
}
