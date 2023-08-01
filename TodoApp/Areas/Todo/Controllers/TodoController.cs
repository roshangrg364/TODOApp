using DomainModule.RepositoryInterface;
using DomainModule.ServiceInterface;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.InteropServices;
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
            foreach(var  todo in todolist)
            {
               
            }
            return View();
        }

    }
}
