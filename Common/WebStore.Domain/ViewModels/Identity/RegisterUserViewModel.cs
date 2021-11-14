using Microsoft.AspNetCore.Mvc;

using System.ComponentModel.DataAnnotations;

namespace WebStore.ViewModels.Identity
{
    public class RegisterUserViewModel
    {
        [Required]
        [Display(Name = "Имя пользователя")]
        [Remote("IsNameFree", "Account")]
        public string UserName { get; set; }

        [Required]
        [Display(Name = "Пароль")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [Display(Name = "Подтверждение пароля")]
        [DataType(DataType.Password)]
        [Compare(nameof(Password))]
        public string PasswordConfirmation { get; set; }

    }
}
