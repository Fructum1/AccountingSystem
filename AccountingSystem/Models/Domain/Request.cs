using System.ComponentModel.DataAnnotations;

namespace AccountingSystem.Models
{
    public partial class Request
    {
        public Request()
        {
            Users = new HashSet<User>();
        }

        public int Id { get; set; }

        public int RequestType { get; set; }

        [DisplayFormat(DataFormatString = "{0:d}")]
        [DataType(DataType.Date)]
        public DateTime? SendingDate { get; set; }

        [DisplayFormat(DataFormatString = "{0:d}")]
        [DataType(DataType.Date)]
        public DateTime CreationDate { get; set; }

        [DisplayFormat(DataFormatString = "{0:d}")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [DisplayFormat(DataFormatString = "{0:d}")]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        public int Sender { get; set; }

        public bool PortabilityDate { get; set; }

        public int Status { get; set; }

        public string? Comment { get; set; }

        public int? SubstituteEmployee { get; set; }

        public virtual RequestType RequestTypeNavigation { get; set; } = null!;

        public virtual User SenderNavigation { get; set; } = null!;

        public virtual RequestStatus StatusNavigation { get; set; } = null!;
        public virtual ICollection<User> Users { get; set; }
    }
}
