using DomainModule.Dto;
using DomainModule.ServiceInterface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ServiceModule.Service;
using TodoApp.Areas.Todo.ViewModel;
using TodoApp.Extensions;
using TodoApp.Models;
using TodoApp.ViewModel;

namespace TodoApp.Areas.Todo.ApiController
{
    [Route("api/todo")]
    [ApiController]
    public class TodoApiController : Controller
    {
        private readonly TodoServiceInterface _todoService;
   
        public TodoApiController(TodoServiceInterface todoService)
        {
            _todoService = todoService;
        }

        [HttpPost("complete")]
        public async Task<IActionResult> MarkAsCompleted([FromBody] TodoHistoryCreateViewModel model)
        {
            try
            {

                var todoHistoryDto = new TodoHistoryCreateDto(this.GetCurrentUserId(), model.TodoId, model.Comment);
                var todoDetails = await _todoService.MarkAsComplete(todoHistoryDto).ConfigureAwait(true);
                ToDoDetailsViewModel todoDetailViewModel = BindTodoDetailViewModel(todoDetails);

                var todoDetailsView = this.RenderViewAsync("~/Areas/Todo/Views/Todo/_todoDetailsPartial.cshtml", todoDetailViewModel,true).GetAwaiter().GetResult();
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
                var todoDetails =await _todoService.CommentOnTodo(todoHistoryDto).ConfigureAwait(true);
                ToDoDetailsViewModel todoDetailViewModel = BindTodoDetailViewModel(todoDetails);

                var todoDetailsView =  this.RenderViewAsync("~/Areas/Todo/Views/Todo/_todoDetailsPartial.cshtml", todoDetailViewModel, true).GetAwaiter().GetResult();
               
                return new JsonResult(new ResponseModel { IsSuccess = true, Status = StatusType.success.ToString(), Message = "Commented On Todo Successfully" ,Data = todoDetailsView});
            }
            catch (Exception ex)
            {
                return new JsonResult(new ResponseModel { IsSuccess = false, Status = StatusType.error.ToString(), Message = ex.Message });
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
                  var todoDetails = await _todoService.SetRemainder(this.GetCurrentUserId(),model.TodoId,model.RemainderOn).ConfigureAwait(true);
                ToDoDetailsViewModel todoDetailViewModel = BindTodoDetailViewModel(todoDetails);

                var todoDetailsView = this.RenderViewAsync("~/Areas/Todo/Views/Todo/_todoDetailsPartial.cshtml", todoDetailViewModel, true).GetAwaiter().GetResult();

                return new JsonResult(new ResponseModel { IsSuccess = true, Status = StatusType.success.ToString(), Message = "Remainder Set Successfully", Data = todoDetailsView });
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
                var todoDetails = await _todoService.UnsetRemainder(this.GetCurrentUserId(),todoId).ConfigureAwait(true);
                ToDoDetailsViewModel todoDetailViewModel = BindTodoDetailViewModel(todoDetails);

                var todoDetailsView = this.RenderViewAsync("~/Areas/Todo/Views/Todo/_todoDetailsPartial.cshtml", todoDetailViewModel, true).GetAwaiter().GetResult();

                return new JsonResult(new ResponseModel { IsSuccess = true, Status = StatusType.success.ToString(), Message = "Remainder Unset Successfully", Data = todoDetailsView });
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
