using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Kindergarten.ViewModels.User
{
    public class UserViewModel
    {
        public string Id { get; set; }

        [EmailAddress(ErrorMessage = "Некорректный адрес")]
        public string Email { get; set; }

        [Display(Name = "Роль")]
        public string RoleName { get; set; }
    }
}
