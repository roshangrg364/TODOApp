using DomainModule.Dto;
using DomainModule.Dto.Todo;
using DomainModule.Entity;
using DomainModule.ServiceInterface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.SignalR;
using ServiceModule.Service;
using TodoApp.Areas.Todo;
using TodoApp.Extensions;
using TodoApp.Models;
using TodoApp.SignalR;
using TodoApp.ViewModel;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace TodoApp.Areas.Todo.ApiController
{
    [Route("api/todo")]
    [ApiController]
    public class TodoApiController : Controller
    {
        private readonly TodoServiceInterface _todoService;
        private readonly IHubContext<TodoHub> _hub;
   
        public TodoApiController(TodoServiceInterface todoService,
            IHubContext<TodoHub> hub)
        {
            _todoService = todoService;
            _hub = hub;
        }

        [HttpPost("complete")]
        public async Task<IActionResult> MarkAsCompleted([FromBody] TodoHistoryCreateViewModel model)
        {
            try
            {
                var currentUser = await this.GetCurrentUser();
                var todoHistoryDto = new TodoHistoryCreateDto(currentUser.Id, model.TodoId, model.Comment);
                var todoCompleteDetails = await _todoService.MarkAsComplete(todoHistoryDto).ConfigureAwait(true);
                ToDoDetailsViewModel todoDetailViewModel = BindTodoDetailViewModel(todoCompleteDetails.TodoDetails);

                var todoDetailsView = this.RenderViewAsync("~/Areas/Todo/Views/Todo/_todoDetailsPartial.cshtml", todoDetailViewModel,true).GetAwaiter().GetResult();

                var todo = await _todoService.GetById(model.TodoId);
                var notificationMessage = $"<div>Todo :(<a href='/Todo/Todo/ViewDetail?todoId={todo.Id}'>{todo.Title}</a>) completed By {currentUser.Name}</div>";
                await BroadCastMessage("CompleteTodo", model.TodoId, todoCompleteDetails.SharedTodoUsersList, notificationMessage, todoDetailsView);
                return new JsonResult(new ResponseModel { IsSuccess = true, Status = StatusType.success.ToString(), Message = "Todo Completed Successfully", Data = todoDetailsView });
            }
            catch (Exception ex)
            {
                return new JsonResult(new ResponseModel { IsSuccess = false, Status = StatusType.error.ToString(), Message = ex.Message });
            }
        }

     
        [HttpPost("comment")]
        public async Task<IActionResult> CommentOnTodo([FromBody] TodoHistoryCreateViewModel model)
        {
            try
            {
                var todoHistoryDto = new TodoHistoryCreateDto(this.GetCurrentUserId(), model.TodoId, model.Comment);
                var SharedTodoUserAndLatestHistory = await _todoService.CommentOnTodo(todoHistoryDto).ConfigureAwait(true);
                var todoHistory = new TodoHistoryViewModel
                {
                    Status = SharedTodoUserAndLatestHistory.LatestTodoHistory.Status,
                    CreatedOn = SharedTodoUserAndLatestHistory.LatestTodoHistory.CreatedOn,
                    Comment = SharedTodoUserAndLatestHistory.LatestTodoHistory.Comment,
                    CommentedBy = SharedTodoUserAndLatestHistory.LatestTodoHistory.CommentedBy,
                };
                var todoCommentView = this.RenderViewAsync("~/Areas/Todo/Views/Todo/_todoCommentPartial.cshtml", todoHistory, true).GetAwaiter().GetResult();

                var todo = await _todoService.GetById(model.TodoId);
                var notificationMessage = $"<div>A new comment added on Todo :(<a href='/Todo/Todo/ViewDetail?todoId={todo.Id}'>{todo.Title}</a>)  By {SharedTodoUserAndLatestHistory.LatestTodoHistory.CommentedBy}</div>";
                await BroadCastMessage("RefreshTodoComment",model.TodoId, SharedTodoUserAndLatestHistory.SharedTodoUsersList,notificationMessage, todoCommentView);

                return new JsonResult(new ResponseModel { IsSuccess = true, Status = StatusType.success.ToString(), Message = "Commented On Todo Successfully", Data = todoCommentView });
            }
            catch (Exception ex)
            {
                return new JsonResult(new ResponseModel { IsSuccess = false, Status = StatusType.error.ToString(), Message = ex.Message });
            }
        }

        private async Task BroadCastMessage(string method, int todoId, List<string> sharedTodoUsersList,string notificationMessage, string message)
        {
            if (sharedTodoUsersList.Any())
            {
                 foreach (var sharedToUser in sharedTodoUsersList)
                {
                    var connectionIdOfSharedUser = TodoHub._connections.GetConnections(sharedToUser).FirstOrDefault();
                    if (!string.IsNullOrEmpty(connectionIdOfSharedUser))
                    {
                        await _hub.Clients.Client(connectionIdOfSharedUser).SendAsync(method, message, notificationMessage, todoId);
                    }
                }
            }
        }

        [HttpDelete("{todoId:int}")]
        public async Task<IActionResult> Delete(int todoId)
        {
            try
            {
                await _todoService.Delete(todoId).ConfigureAwait(true);
                return new JsonResult(new ResponseModel { IsSuccess = true, Status = StatusType.success.ToString(), Message = "Deleted Successfully"});
            }
            catch (Exception ex)
            {
                return new JsonResult(new ResponseModel { IsSuccess = false, Status = StatusType.error.ToString(), Message = ex.Message });
            }
        }

        [HttpPost("set-remainder")]
        public async Task<IActionResult> SetRemainder([FromBody] TodoRemainderViewModel model)
        {
            try
            {
                   await _todoService.SetRemainder(this.GetCurrentUserId(),model.TodoId,model.RemainderOn).ConfigureAwait(true);
               
                return new JsonResult(new ResponseModel { IsSuccess = true, Status = StatusType.success.ToString(), Message = "Remainder Set Successfully"});
            }
            catch (Exception ex)
            {
                return new JsonResult(new ResponseModel { IsSuccess = false, Status = StatusType.error.ToString(), Message = ex.Message });
            }
        }

        [HttpPut("unset-remainder/{todoId:int}")]
        public async Task<IActionResult> UnSetRemainder(int todoId)
        {
            try
            {
                await _todoService.UnsetRemainder(this.GetCurrentUserId(),todoId).ConfigureAwait(true);
               
                return new JsonResult(new ResponseModel { IsSuccess = true, Status = StatusType.success.ToString(), Message = "Remainder Unset Successfully" });
            }
            catch (Exception ex)
            {
                return new JsonResult(new ResponseModel { IsSuccess = false, Status = StatusType.error.ToString(), Message = ex.Message });
            }
        }

        [HttpGet("todo-remainder")]
        public async Task<IActionResult> TodoRemainder()
        {
            try
            {
                var messageList = await _todoService.GetTodoRemainder(this.GetCurrentUserId()).ConfigureAwait(true); 
                if(!messageList.Any())
                {
                    return new JsonResult(new ResponseModel { IsSuccess = false, Status = StatusType.info.ToString() });
                }
                else
                {
                    var todoDetailsView = this.RenderViewAsync("~/Views/_RemainderNotification.cshtml", messageList, true).GetAwaiter().GetResult();
                    return new JsonResult(new ResponseModel { IsSuccess = true, Status = StatusType.success.ToString(), Data = todoDetailsView });

                }
            }
            catch (Exception ex)
            {
                return new JsonResult(new ResponseModel { IsSuccess = false, Status = StatusType.error.ToString(), Message = ex.Message });
            }
        }

     
        private static ToDoDetailsViewModel BindTodoDetailViewModel(TodoDetailsDto todoDetails)
        {
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
                IsEligible = todoDetails.IsEligible,    
                HasRemainderSet = todoDetails.HasSetRemainder
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

            return todoDetailViewModel;
        }


    }
}
