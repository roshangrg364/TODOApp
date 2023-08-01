namespace TodoApp.ViewModel
{
    public class TodoIndexViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Status { get; set; }
        public string PriorityLevel { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? CompletedOn { get; set; }
        public DateTime DueDate { get; set; }
        public string? Description { get; set; }
        public string CreatedByUser { get; set; }
        public string? CompletedByUser { get; set; }

    }
}
