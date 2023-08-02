using DomainModule.Dto;
using DomainModule.Enums;
using DomainModule.ServiceInterface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using TodoApp.Extensions;
using TodoApp.ViewModel;

namespace TodoApp.Areas.Todo.Controllers
{
    [Area("Todo")]
    public class SharedTodoController : Controller
    {
        private readonly TodoServiceInterface _todoService;
        private readonly SharedTodoServiceInterface _sharedTodoService;
        public SharedTodoController(TodoServiceInterface todoService,
            SharedTodoServiceInterface sharedTodoService)
        {
            _todoService = todoService;
            _sharedTodoService = sharedTodoService;
        }
        public async Task<IActionResult> Index(SharedTodoIndexViewModel model)
        {
            var todoFilterModel = new SharedTodoFilterDto
            {
                CurrentUserId = this.GetCurrentUserId(),
                FromDate = model.FromDate,
                ToDate = model.ToDate,
                PriorityLevel = model.PriorityLevel,
                Status = model.Status,
                Title = model.Title
            };
            var sharedTodos = await _sharedTodoService.GetAllTodosSharedToCurrentUser(todoFilterModel).ConfigureAwait(false);

            SetPriorityLevelViewBag();
            foreach (var todo in sharedTodos)
            {
                model.Todos.Add(new SharedTodoViewModel
                {
                    Id = todo.Id,
                    TodoId = todo.TodoId,
                    TodoTitle = todo.TodoTitle,
                    PriorityLevel = todo.PriorityLevel,
                    Description = todo.Description,
                    TodoDueDate = todo.TodoDueDate,
                    Status = todo.Status,
                    SharedBy = todo.SharedBy,
                    SharedTo = todo.SharedTo
                });
            }
            return View(model);
        }

        private void SetPriorityLevelViewBag()
        {
            var todoPriorityLevels = from TodoPriorityEnum s in Enum.GetValues(typeof(TodoPriorityEnum)) select new { Id = (int)s, DisplayName = s.ToString() };
            ViewBag.TodoPriorityLevel = new SelectList(todoPriorityLevels, "Id", "DisplayName");
        }
    }
}
