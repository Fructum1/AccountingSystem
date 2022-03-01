using System.ComponentModel.DataAnnotations;

namespace AccountingSystem.Models.ViewModels
{
    public class RequestDetailsViewModel
    {
        private RequestDetailsViewModel(Request request)
        {
            Id = request.Id;
            RequestType = request.RequestType;
            SendingDate = request.SendingDate;
            CreationDate = request.CreationDate;
            StartDate = request.StartDate;
            EndDate = request.EndDate;
            Sender = request.Sender;
            SenderNavigation = request.SenderNavigation;
            PortabilityDate = request.PortabilityDate;
            Status = request.Status;
            Comment = request.Comment;
            RequestTypeNavigation = request.RequestTypeNavigation;  
            StatusNavigation = request.StatusNavigation;
            Users = request.Users;
        }
        public static RequestDetailsViewModel FromRequest(Request request)
        {
            return new RequestDetailsViewModel(request);
        }
        public int Id { get; set; }

        [Display(Name = "Тип заявки")]
        public int RequestType { get; set; }

        [Display(Name = "Дата отправки")]
        [DisplayFormat(DataFormatString = "{0:d}")]
        [DataType(DataType.Date)]
        public DateTime? SendingDate { get; set; }

        [DisplayFormat(DataFormatString = "{0:d}")]
        [Display(Name = "Дата создания")]
        [DataType(DataType.Date)]
        public DateTime CreationDate { get; set; }

        [DisplayFormat(DataFormatString = "{0:d}")]
        [Display(Name = "Дата начала")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [DisplayFormat(DataFormatString = "{0:d}")]
        [Display(Name = "Дата конца")]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        [Display(Name = "Отправитель")]
        public int Sender { get; set; }

        [Display(Name = "Переносимость даты")]
        public bool PortabilityDate { get; set; }

        [Display(Name = "Статус")]
        public int Status { get; set; }

        [Display(Name = "Комментарий")]
        public string? Comment { get; set; }

        [Display(Name = "Заменяющий сотрудник")]
        public string? SubstituteEmployee { get; set; }

        [Display(Name = "Тип заявки")]
        public virtual RequestType RequestTypeNavigation { get; set; } = null!;

        [Display(Name = "Отправитель")]
        public virtual User SenderNavigation { get; set; } = null!;

        [Display(Name = "Статус")]
        public virtual RequestStatus StatusNavigation { get; set; } = null!;
        [Display(Name = "Согласовали")]
        public virtual ICollection<User>? Users { get; set; }
    }
}
