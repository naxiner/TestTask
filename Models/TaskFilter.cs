namespace TestTask.Models
{
    public class TaskFilter
    {
        public DateTime? DueDate { get; set; }
        public Status? Status { get; set; }
        public Priority? Priority { get; set; }
        public string? SortBy { get; set; }
        public bool? SortDescending { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
