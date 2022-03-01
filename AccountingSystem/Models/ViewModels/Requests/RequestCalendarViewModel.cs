using Microsoft.AspNetCore.Mvc.Rendering;


namespace AccountingSystem.Models.ViewModels
{
    public class RequestCalendarViewModel
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Start { get; set; }
        public string? End { get; set; }
        public string? Status { get; set; }

    }
}
