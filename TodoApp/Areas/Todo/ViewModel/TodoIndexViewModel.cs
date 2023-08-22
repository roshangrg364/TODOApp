namespace TodoApp.ViewModel
{
    public class TodoIndexViewModel:TodoFilterModel
    {
        public TodoIndexViewModel()
        {
            FromDate = DateTime.Now.AddDays(-10);
            ToDate = DateTime.Now.AddDays(20);
        }
       
        public IList<TodoViewModel> Todos { get; set; } = new List<TodoViewModel>();
    }

    public class TodoFilterModel
    {
        public string? Title { get; set; } 
        public string? Title123 { get; set; } 
        public string? Status { get; set; } 
        public string currentUserId { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public int? PriorityLevel { get; set; }
    }

    public class TodoViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Status { get; set; }
        public bool IsTodoCreator { get; set; }
        public string PriorityLevel { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? CompletedOn { get; set; }
        public DateTime DueDate { get; set; }
        public string? Description { get; set; }
        public string CreatedByUser { get; set; }
        public string? CompletedByUser { get; set; }
    }

}
