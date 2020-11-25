using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Kindergarten.ViewModels.User
{
    public class CreateUserViewModel
    {
        [Display(Name = "Имя")]
        public string UserName { get; set; }
        [EmailAddress(ErrorMessage = "Некорректный адрес")]
        public string Email { get; set; }
        [Display(Name = "Пароль")]
        public string Password { get; set; }

        [Display(Name = "Роль")]
        public string UserRole { get; set; }
        public CreateUserViewModel()
        {
            UserRole = "User";
        }
    }
}
