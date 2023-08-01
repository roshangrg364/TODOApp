using DomainModule.Dto;
using DomainModule.Enums;
using DomainModule.RepositoryInterface;
using DomainModule.ServiceInterface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data;
using System.Runtime.InteropServices;
using TodoApp.Areas.Todo.ViewModel;
using TodoApp.Extensions;
using TodoApp.ViewModel;

namespace TodoApp.Areas.Todo.Controllers
{
    [Area("Todo")]
    public class TodoController : Controller
    {
        private readonly ILogger<TodoController> _logger;
        private readonly TodoServiceInterface _todoService;

        public TodoController(ILogger<TodoController> logger,
            TodoServiceInterface todoService)
        {
            _logger = logger;
            _todoService = todoService;
        }
        public async Task<IActionResult> Index()
        {
            var currentUSer = this.GetCurrentUserId();
            var todolist = await _todoService.GetAllTodosOfUser(currentUSer).ConfigureAwait(true);
            var todoIndexModel = new List<TodoIndexViewModel>();
            foreach (var todo in todolist)
            {
                todoIndexModel.Add(new TodoIndexViewModel
                {
                    Id = todo.Id,
                    Title=todo.Title,
                    PriorityLevel= todo.PriorityLevel,
                    Description = todo.Description,
                    DueDate = todo.DueDate,
                    Status=todo.Status,
                    CompletedByUser= todo.CompletedByUser,
                    CreatedByUser=todo.CreatedByUser,
                    CompletedOn=todo.CompletedOn,
                    CreatedOn =todo.CreatedOn
                });
            }
            return View(todoIndexModel);
        }

        public Task<IActionResult> Create()
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
                if(ModelState.IsValid)
                {
                    var todoCreateDto = new TodoCreateDto(model.Title, model.PriorityLevel, model.DueDate, this.GetCurrentUserId())
                    { 
                    Description = model.Description
                    };
                    await _todoService.Create(todoCreateDto).ConfigureAwait(true);
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


        public async Task<IActionResult> Udpate(int todoId)
        {
            try
            {
                SetPriorityLevelViewBag();
                var todo = await _todoService.GetById(todoId).ConfigureAwait(true);
                var todoEditViewModel = new TodoEditViewModel() { 
                Id = todoId,
                PriorityLevel = todo.PriorityLevel,
                Description =todo.Description,
                DueDate = todo.DueDate,
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
                    var todoEditDto = new TodoEditDto(model.Id,model.Title, model.PriorityLevel, model.DueDate)
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
    }
}
