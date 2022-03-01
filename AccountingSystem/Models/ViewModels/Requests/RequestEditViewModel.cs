using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace AccountingSystem.Models.ViewModels
{
    public class RequestEditViewModel
    {
        public RequestEditViewModel()
        {
            Users = new HashSet<User>();
        }
        public int Id { get; set; }
        [DisplayFormat(DataFormatString = "{0:d}")]
        [Display(Name = "Дата начала")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }
        [DisplayFormat(DataFormatString = "{0:d}")]
        [Display(Name = "Дата конца")]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }
        [Display(Name = "Комментарий")]
        public string? Comment { get; set; }
        public int? SubstituteEmployee { get; set; }
        [Display(Name = "Переносимость даты")]
        public bool PortabilityDate { get; set; }
        public int RequestType {get; set;}
        [Display(Name = "Тип заявки")]
        public virtual RequestType RequestTypeNavigation { get; set; } = null!;
        [Display(Name = "Согласовали")]
        public virtual ICollection<User>? Users { get; set; }
        [Display(Name = "Замещающие")]
        public SelectList UsersForSubstitute { get; set; } = null!;
        [Display(Name = "Согласующие")]
        public SelectList UsersForApprove { get; set; } = null!;
        [Display(Name = "Тип заявки")]
        public SelectList RequestTypes { get; set; } = null!;
    }
}
