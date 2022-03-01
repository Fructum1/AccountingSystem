using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace AccountingSystem.Models.ViewModels
{
    public class UserCreateViewModel
    {
        public UserCreateViewModel()
        {
            Roles = new HashSet<Role>();
        }
        public int Id { get; set; }

        [Display(Name = "Имя")]
        [StringLength(500, MinimumLength = 3)]
        public string Name { get; set; } = null!;

        [Display(Name = "Фамилия")]
        [StringLength(500, MinimumLength = 4)]
        public string Surname { get; set; } = null!;

        [Display(Name = "Отчество")]
        [StringLength(500)]
        public string? Patronymic { get; set; }
        [Display(Name = "Пароль")]
        [StringLength(100, MinimumLength = 8)]
        public string Password { get; set; } = null!;

        [Display(Name = "Должность")]
        public int Position { get; set; }

        [Display(Name = "Номер телефона")]
        [StringLength(15)]
        public string? PhoneNumber { get; set; }

        [Display(Name = "Почтовый ящик")]
        [Remote(action: "CheckEmail", controller: "Users", ErrorMessage = "Email уже используется")]

        [StringLength(150)]
        public string Email { get; set; } = null!;

        [Display(Name = "Отдел")]
        public int Unit { get; set; }

        [Display(Name = "Роли")]
        public virtual ICollection<Role> Roles { get; set; }
    }
}
