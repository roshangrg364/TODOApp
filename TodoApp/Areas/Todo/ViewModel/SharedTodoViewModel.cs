using TodoApp.Areas.Todo.ViewModel;

namespace TodoApp.ViewModel
{

    public class SharedTodoIndexViewModel:SharedTodoFilterModel
    {
        public SharedTodoIndexViewModel()
        {
            FromDate = DateTime.Now.AddDays(-10);
            ToDate = DateTime.Now.AddDays(20);
        }

        public IList<SharedTodoViewModel> Todos { get; set; } = new List<SharedTodoViewModel>();
    }

    public class SharedTodoFilterModel {
        public string? Title { get; set; }
        public string? Status { get; set; }
        public string currentUserId { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public int? PriorityLevel { get; set; }
    }

    public class SharedTodoViewModel
    {
        public int Id { get; set; }
        public int TodoId { get; set; }
        public string TodoTitle { get; set; }
        public string Status { get; set; }
        public string PriorityLevel { get; set; }
        public string? Description { get; set; }
        public DateTime TodoDueDate { get; set; }
        public string SharedBy { get; set; }
        public string SharedTo { get; set; }
        public DateTime CreatedOn { get; set; }
    }

    public class SharedTodoCreateViewModel
    {
      
        public string Email { get; set; }
        public int TodoId { get; set; }
        public string Description { get; set; }

    }

    public class ToDoDetailsViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string CreatedBy { get; set; }
        public bool IsTodoCreator { get; set; }
        public bool IsEligible { get; set; }
        public bool HasRemainderSet { get; set; }
        public string? Description { get; set; }
        public string PriorityLevel { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string Status { get; set; }
        public DateTime? CompletedOn { get; set; }
        public string CompletedBy { get; set; }
        public List<TodoHistoryViewModel> SharedTodoHistory { get; set; } = new List<TodoHistoryViewModel>();
    }
}
