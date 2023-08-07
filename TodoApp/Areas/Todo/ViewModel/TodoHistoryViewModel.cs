namespace TodoApp.ViewModel
{
    public class TodoHistoryViewModel
    {
        public int Id { get; set; }
        public string? Comment { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CommentedBy { get; set; }
        public string Status { get; set; }
    }
}
