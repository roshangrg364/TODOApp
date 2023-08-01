using DomainModule.Dto;
using DomainModule.Entity;
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
        private readonly TodoRepositoryInterface _todoRepo;
        private readonly UserRepositoryInterface _userRepo;
        private readonly IUnitOfWork _unitOfWork;
        public SharedTodoService(SharedTodoRepositoryInterface sharedTodoRepo,
            TodoRepositoryInterface todoRepo,
            IUnitOfWork unitOfWork,
            UserRepositoryInterface userRepo)
        {
            _sharedTodoRepo = sharedTodoRepo;
            _todoRepo = todoRepo;
            _unitOfWork = unitOfWork;
            _userRepo = userRepo;
        }
        public async Task Create(SharedTodoCreateDto dto)
        {
            using (var tx = await _unitOfWork.BeginTransaction(System.Data.IsolationLevel.ReadCommitted))
            {
                try
                {
                    var todo = await _todoRepo.GetById(dto.TodoId).ConfigureAwait(false) ?? throw new CustomException("Todo does not exists.");
                    var user = await _userRepo.GetQueryable().Where(a => a.Email == dto.Email).FirstOrDefaultAsync().ConfigureAwait(false) ?? throw new CustomException("User not found");

                    var sharedTodoEntity = new SharedTodoEntity(todo, user);
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

        public async Task<SharedTodoOfTodo> GetSharedTodosListOfTodo(int todoId)
        {
            try
            {
                var todo = await _todoRepo.GetById(todoId).ConfigureAwait(false) ?? throw new CustomException("Todo not found");

                var sharedTodoReturnModel = new SharedTodoOfTodo()
                {
                    Title = todo.Title,
                    CreatedBy = todo.CreatedByUser.Name,
                    Description = todo.Description
                };
               
                foreach (var sharedTodo in todo.SharedTodos)
                {
                    var sharedTodoModel = new SharedTodoDto
                    {
                        Id = sharedTodo.Id,
                        SharedTo = sharedTodo.User.Name,
                        Description = sharedTodo.Description
                    };
                    sharedTodoReturnModel.SharedTodos.Add(sharedTodoModel);
                }
                return sharedTodoReturnModel;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task<List<SharedTodoDto>> GetAllTodosSharedToCurrentUser(string currentUserId)
        {
            try
            {
                var user = await _userRepo.GetByIdString(currentUserId).ConfigureAwait(false) ?? throw new UserNotFoundException();
                var sharedTodosOfUser = await _sharedTodoRepo.GetQueryable().Where(a => a.UserId == currentUserId).ToListAsync().ConfigureAwait(false);
                var returnModel = new List<SharedTodoDto>();
                foreach(var  sharedTodo in sharedTodosOfUser)
                {
                    var model = new SharedTodoDto
                    {
                        Id = sharedTodo.Id,
                        TodoId = sharedTodo.TodoId,
                        TodoTitle = sharedTodo.Todo.Title,
                        Description = sharedTodo.Description,
                        TodoDueDate = sharedTodo.Todo.DueDate,
                        SharedBy = sharedTodo.Todo.CreatedByUser.Name,
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
    }
}
