using DomainModule.ServiceInterface;
using Microsoft.AspNetCore.SignalR;

namespace TodoApp.SignalR
{
    public class TodoHub : Hub
    {
        private readonly TodoServiceInterface _todoService;
        public TodoHub(TodoServiceInterface todoService)
        {
            _todoService = todoService;
        }
        public async Task ShareTodo(int todoId)
        {
            var todo = await _todoService.GetById(todoId);
            await Clients.Others.SendAsync("ReceiveMessage", todo.CreatedBy,"TodoCreated");
        }
    }
}