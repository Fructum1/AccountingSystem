using System.ComponentModel.DataAnnotations;

namespace AccountingSystem.Models
{
    public class RequestListViewModel
    {
        [Display(Name = "Номер заявки")]
        public int Id { get; set; }
        [Display(Name = "Имя")]
        public string? Name { get; set; }
        [Display(Name = "Фамилия")]
        public string? Surname { get; set; }
        [Display(Name = "Отдел")]
        public string? Unit { get; set; }
        [Display(Name = "Дата создания")]
        [DisplayFormat(DataFormatString = "{0:d}")]
        public DateTime CreationDate { get; set; }
        [Display(Name = "Статус")]
        public int Status { get; set; }
        [Display(Name = "Тип заявки")]
        public virtual RequestType RequestTypeNavigation { get; set; } = null!;
        [Display(Name = "Дата начала")]
        [DisplayFormat(DataFormatString = "{0:d}")]
        public DateTime StartDate { get; set; }
        [Display(Name = "Дата окончания")]
        [DisplayFormat(DataFormatString = "{0:d}")]
        public DateTime EndDate { get; set; }
        [Display(Name = "Согласующие")]
        public virtual ICollection<User>? Users { get; set; }
        [Display(Name = "Статус")]
        public virtual RequestStatus StatusNavigation { get; set; } = null!;
    }
}
