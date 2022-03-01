using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AccountingSystem.Models
{
    public partial class User
    {
        public User()
        {
            Requests = new HashSet<Request>();
            RequestsNavigation = new HashSet<Request>();
            Roles = new HashSet<Role>();
        }

        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public string Surname { get; set; } = null!;

        public string? Patronymic { get; set; }

        public string Password { get; set; } = null!;
     
        public int Position { get; set; }

        public string? PhoneNumber { get; set; }

        public string Email { get; set; } = null!;
   
        public int Unit { get; set; }
        public int UnusedVacationDays { get; set; } = 20;

        public virtual Position PositionNavigation { get; set; } = null!;
        public virtual Unit UnitNavigation { get; set; } = null!;

        public virtual ICollection<Request> Requests { get; set; }

        public virtual ICollection<Request> RequestsNavigation { get; set; }

        public virtual ICollection<Role> Roles { get; set; }
    }
}
