using DomainModule.Dto;
using DomainModule.Entity;
using DomainModule.Enums;
using DomainModule.RepositoryInterface;
using DomainModule.ServiceInterface;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.SignalR;
using NToastNotify;
using System.Data;
using TodoApp.Areas.Todo.ViewModel;
using TodoApp.Extensions;
using TodoApp.SignalR;
using TodoApp.ViewModel;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace TodoApp.Areas.Todo.Controllers
{
    [Area("Todo")]
    public class TodoController : Controller
    {
        private readonly ILogger<TodoController> _logger;
        private readonly TodoServiceInterface _todoService;
        private readonly SharedTodoServiceInterface _sharedTodoService;
        private readonly IToastNotification _notify;
        private readonly UserManager<User> _userManager;

        private readonly IHubContext<TodoHub> _hub;
        public TodoController(ILogger<TodoController> logger,
            TodoServiceInterface todoService, IToastNotification notify,
            SharedTodoServiceInterface sharedTodoService,
            IHubContext<TodoHub> hub, UserManager<User> userManager)
        {
            _logger = logger;
            _todoService = todoService;
            _notify = notify;
            _sharedTodoService = sharedTodoService;
            _hub = hub;
            _userManager = userManager;
        }
        public async Task<IActionResult> Index(TodoIndexViewModel model)
        {
            var currentUSerId = this.GetCurrentUserId();
            TodoFilterDto todoFilterModel = BindDataToTodoFilterDto(model, currentUSerId, model.Status);
            var todolist = await _todoService.GetAllTodosOfUser(todoFilterModel).ConfigureAwait(true);
            SetPriorityLevelViewBag();
            foreach (var todo in todolist)
            {
                model.Todos.Add(new TodoViewModel
                {
                    Id = todo.Id,
                    Title = todo.Title,
                    PriorityLevel = todo.PriorityLevel,
                    Description = todo.Description,
                    DueDate = todo.DueDate,
                    Status = todo.Status,
                    CompletedByUser = todo.CompletedByUser,
                    CreatedByUser = todo.CreatedByUser,
                    CompletedOn = todo.CompletedOn,
                    CreatedOn = todo.CreatedOn,
                    IsTodoCreator = todo.CreatedBy == currentUSerId
                });
            }
            return View(model);
        }

        public async Task<IActionResult> ActiveTodo(TodoIndexViewModel model)
        {
            var currentUSerId = this.GetCurrentUserId();
            TodoFilterDto todoFilterModel = BindDataToTodoFilterDto(model, currentUSerId, TodoEntity.StatusActive);
            var todolist = await _todoService.GetAllTodosOfUser(todoFilterModel).ConfigureAwait(true);
            SetPriorityLevelViewBag();
            foreach (var todo in todolist)
            {
                model.Todos.Add(new TodoViewModel
                {
                    Id = todo.Id,
                    Title = todo.Title,
                    PriorityLevel = todo.PriorityLevel,
                    Description = todo.Description,
                    DueDate = todo.DueDate,
                    Status = todo.Status,
                    CompletedByUser = todo.CompletedByUser,
                    CreatedByUser = todo.CreatedByUser,
                    CompletedOn = todo.CompletedOn,
                    CreatedOn = todo.CreatedOn
                });
            }
            return View(model);
        }


        public async Task<IActionResult> CompletedTodo(TodoIndexViewModel model)
        {
            var currentUSerId = this.GetCurrentUserId();
            TodoFilterDto todoFilterModel = BindDataToTodoFilterDto(model, currentUSerId, TodoEntity.StatusCompleted);

            var todolist = await _todoService.GetAllTodosOfUser(todoFilterModel).ConfigureAwait(true);
            SetPriorityLevelViewBag();
            foreach (var todo in todolist)
            {
                model.Todos.Add(new TodoViewModel
                {
                    Id = todo.Id,
                    Title = todo.Title,
                    PriorityLevel = todo.PriorityLevel,
                    Description = todo.Description,
                    DueDate = todo.DueDate,
                    Status = todo.Status,
                    CompletedByUser = todo.CompletedByUser,
                    CreatedByUser = todo.CreatedByUser,
                    CompletedOn = todo.CompletedOn,
                    CreatedOn = todo.CreatedOn
                });
            }
            return View(model);
        }


        private static TodoFilterDto BindDataToTodoFilterDto(TodoIndexViewModel model, string currentUSerId, string status)
        {
            return new TodoFilterDto
            {
                CurrentUserId = currentUSerId,
                FromDate = model.FromDate,
                ToDate = model.ToDate,
                PriorityLevel = model.PriorityLevel,
                Status = status,
                Title = model.Title
            };
        }

        public IActionResult Create()
        {
            SetPriorityLevelViewBag();
            return View();
        }

        private void SetPriorityLevelViewBag()
        {
            var todoPriorityLevels = from TodoPriorityEnum s in Enum.GetValues(typeof(TodoPriorityEnum)) select new { Id = (int)s, DisplayName = s.ToString() };
            ViewBag.TodoPriorityLevel = new SelectList(todoPriorityLevels, "Id", "DisplayName");
        }

        [HttpPost]
        public async Task<IActionResult> Create(TodoCreateViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var todoCreateDto = new TodoCreateDto(model.Title, model.PriorityLevel, model.DueDate, this.GetCurrentUserId())
                    {
                        Description = model.Description
                    };
                    var todoId = await _todoService.Create(todoCreateDto).ConfigureAwait(true);
                    _notify.AddSuccessToastMessage("Todo Created Successfully");
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    var validationErrors = ModelState.SelectMany(error => error.Value.Errors.Select(message => message.ErrorMessage)).ToList();
                    string responseMessage = string.Join("</br>", validationErrors);
                    _notify.AddInfoToastMessage(responseMessage);
                }

            }
            catch (Exception ex)
            {
                _notify.AddErrorToastMessage(ex.Message);

            }
            SetPriorityLevelViewBag();
            return View(model);
        }


        public async Task<IActionResult> Update(int todoId)
        {
            try
            {
                SetPriorityLevelViewBag();
                var todo = await _todoService.GetById(todoId).ConfigureAwait(true);
                var todoEditViewModel = new TodoEditViewModel()
                {
                    Id = todoId,
                    PriorityLevel = todo.PriorityLevel,
                    Description = todo.Description,
                    DueDate = todo.DueDate,
                    Title =todo.Title
                };

                return View(todoEditViewModel);

            }
            catch (Exception ex)
            {

                _notify.AddErrorToastMessage(ex.Message);
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        public async Task<IActionResult> Update(TodoEditViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var todoEditDto = new TodoEditDto(model.Id, model.Title, model.PriorityLevel, model.DueDate)
                    {
                        Description = model.Description
                    };
                    await _todoService.Update(todoEditDto).ConfigureAwait(true);
                    _notify.AddSuccessToastMessage("Todo Updated Successfully");
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    var validationErrors = ModelState.SelectMany(error => error.Value.Errors.Select(message => message.ErrorMessage)).ToList();
                    string responseMessage = string.Join("</br>", validationErrors);
                    _notify.AddInfoToastMessage(responseMessage);
                }
            }
            catch (Exception ex)
            {
                _notify.AddErrorToastMessage(ex.Message);

            }
            SetPriorityLevelViewBag();
            return View(model);
        }

        public async Task<IActionResult> Delete(int todoId)
        {
            try
            {
                await _todoService.Delete(todoId).ConfigureAwait(true);
                _notify.AddSuccessToastMessage("Todo Deleted Successfully");
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {

                _notify.AddErrorToastMessage(ex.Message);
                return RedirectToAction(nameof(Index));
            }
        }


        public async Task<IActionResult> ViewDetail(int todoId)
        {
            try
            {
                var todoDetails = await _todoService.GetTodoDetails(todoId).ConfigureAwait(true);
                var todoDetailViewModel = new ToDoDetailsViewModel
                {

                    Id = todoDetails.Id,
                    Title = todoDetails.Title,
                    CreatedBy = todoDetails.CreatedBy,
                    Description = todoDetails.Description,
                    PriorityLevel = todoDetails.PriorityLevel,
                    DueDate = todoDetails.DueDate,
                    CreatedOn = todoDetails.CreatedOn,
                    CompletedBy = todoDetails.CompletedBy,
                    CompletedOn = todoDetails.CompletedOn,
                    ModifiedOn = todoDetails.ModifiedOn,
                    Status = todoDetails.Status,
                    IsTodoCreator = todoDetails.CreatedByUserId == this.GetCurrentUserId(),
                };

                foreach (var sharedTodo in todoDetails.SharedTodoHistory)
                {
                    var sharedTodoModel = new TodoHistoryViewModel
                    {
                        Id = sharedTodo.Id,
                        Comment = sharedTodo.Comment,
                        CommentedBy = sharedTodo.CommentedBy,
                        Status = sharedTodo.Status,
                        CreatedOn = sharedTodo.CreatedOn
                    };
                    todoDetailViewModel.SharedTodoHistory.Add(sharedTodoModel);
                }
                return View(todoDetailViewModel);
            }
            catch (Exception ex)
            {

                _notify.AddErrorToastMessage(ex.Message);
                return RedirectToAction(nameof(Index));
            }
        }

        public async Task<IActionResult> ShareTodo(int todoId)
        {
            try
            {
                var sharedToDoModel = new SharedTodoCreateViewModel
                {
                    TodoId = todoId,
                };
                return View(sharedToDoModel);
            }
            catch (Exception ex)
            {
                _notify.AddErrorToastMessage(ex.Message);
                return RedirectToAction(nameof(Index));
            }
        }
        [HttpPost]
        public async Task<IActionResult> ShareTodo(SharedTodoCreateViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var currentUser = await this.GetCurrentUser();
                    var sharedTodoCreateDto = new SharedTodoCreateDto(model.Email, model.TodoId, model.Description, currentUser.Id);
                    await _sharedTodoService.Create(sharedTodoCreateDto);
                    _notify.AddSuccessToastMessage("Todo shared Successfully");
                    var connections = TodoHub._connections;
                    var sharedToUser = await _userManager.FindByEmailAsync(model.Email).ConfigureAwait(false);
                    var message = $"Hello, {sharedToUser.Name} a todo have been shared to you by {currentUser.Name}";
                    await BroadCastMessage(connections, sharedToUser, message);
                    return RedirectToAction(nameof(ViewDetail), new { todoId = model.TodoId });
                }
                else
                {
                    var validationErrors = ModelState.SelectMany(error => error.Value.Errors.Select(message => message.ErrorMessage)).ToList();
                    string responseMessage = string.Join("</br>", validationErrors);
                    _notify.AddInfoToastMessage(responseMessage);
                }
            }
            catch (Exception ex)
            {
                _notify.AddErrorToastMessage(ex.Message);
            }
            return View(model);
        }

        private async Task BroadCastMessage(ConnectionMapping<string> connections, User? sharedToUser, string message)
        {
            var connectionIdOfSharedUser = connections.GetConnections(sharedToUser.UserName).FirstOrDefault();
            if (!string.IsNullOrEmpty(connectionIdOfSharedUser))
            {
                await _hub.Clients.Client(connectionIdOfSharedUser).SendAsync("NotifyUserAndRefreshDashboard", message);
            }
        }
    }
}
