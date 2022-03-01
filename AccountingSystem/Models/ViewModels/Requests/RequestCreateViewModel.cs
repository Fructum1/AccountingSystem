using System.ComponentModel.DataAnnotations;

namespace AccountingSystem.Models.ViewModels
{
    public class RequestCreateViewModel
    {

        public int Id { get; set; }

        [Required(ErrorMessage = "Не указан тип заявки")]
        [Display(Name = "Тип заявки")]
        public int? RequestType { get; set; }

        [DisplayFormat(DataFormatString = "{0:d}")]
        [DataType(DataType.Date)]
        [Display(Name = "Дата начала")]
        [Required(ErrorMessage = "Не указан тип заявки")]
        public DateTime StartDate { get; set; }

        [DisplayFormat(DataFormatString = "{0:d}")]
        [DataType(DataType.Date)]
        [Display(Name = "Дата конца")]
        public DateTime EndDate { get; set; }
        [Display(Name = "Переносимость даты")]
        public bool PortabilityDate { get; set; }
        [Display(Name = "Комментарий")]
        public string? Comment { get; set; }
        [Display(Name = "Замещающий сотрудник")]
        public int? SubstituteEmployee { get; set; }
    }
}
