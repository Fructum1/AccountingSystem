using System.ComponentModel.DataAnnotations;

namespace AccountingSystem.Models.ViewModels
{
    public class UserInfoViewModel
    {
        private UserInfoViewModel(User user)
        {
            Id = user.Id;
            Name = user.Name;
            Surname = user.Surname;
            Patronymic = user.Patronymic;
            Position = user.Position;
            PositionNavigation = user.PositionNavigation;
            Roles = user.Roles;
            Unit = user.Unit;
            UnitNavigation = user.UnitNavigation;
            PhoneNumber = user.PhoneNumber;
            Email = user.Email;
            UnusedVacationDays = user.UnusedVacationDays;
        }

        public static UserInfoViewModel FromUser(User user) 
        {
            return new UserInfoViewModel(user);
        }

        public int Id { get; set; }

        [Display(Name = "Имя")]
        public string Name { get; set; } = null!;

        [Display(Name = "Фамилия")]
        public string Surname { get; set; } = null!;

        [Display(Name = "Отчество")]
        public string? Patronymic { get; set; }

        [Display(Name = "Должность")]
        public int Position { get; set; }

        [Display(Name = "Номер телефона")]
        public string? PhoneNumber { get; set; }

        [Display(Name = "Почтовый ящик")]
        public string Email { get; set; } = null!;

        [Display(Name = "Отдел")]
        public int Unit { get; set; }

        [Display(Name = "Должность")]
        public virtual Position PositionNavigation { get; set; } = null!;

        [Display(Name = "Отдел")]
        public virtual Unit UnitNavigation { get; set; } = null!;

        //public int DaysWorked { get; set; }
        [Display(Name = "Неиспользванные дни отпуска в текущем году")]
        public int UnusedVacationDaysCurrentYear { get; set; }
        [Display(Name = "Неиспользванные дни отпуска")]
        public int UnusedVacationDays { get; set; }
        [Display(Name = "Количество дней отгулов по утвержденным заявкам в текущем году")]
        public int DaysOff { get; set; }

        [Display(Name = "Роли")]
        public virtual ICollection<Role>? Roles { get; set; }
        public List<RequestsByStatusViewModel>? RequestsGrouping { get; set; }
    }
}
