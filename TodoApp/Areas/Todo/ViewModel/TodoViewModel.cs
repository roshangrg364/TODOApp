using System.ComponentModel.DataAnnotations;

namespace TodoApp.ViewModel
{
    public class TodoCreateViewModel
    {
        [Required(ErrorMessage ="Title is required")]
        public string Title { get; set; }
        [Required(ErrorMessage = "Priority Level is required")]
        public int PriorityLevel { get; set; }
        [Required(ErrorMessage = "Due date is required")]
        public DateTime DueDate { get; set; }
        public string? Description { get; set; }
    }

    public class TodoEditViewModel : TodoCreateViewModel
    {
        public int Id { get; set; }
    }

    public class TodoHistoryCreateViewModel {
        public int TodoId { get; set; }
        public string Comment { get; set; }
    }

}
