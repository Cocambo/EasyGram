using System.ComponentModel.DataAnnotations;

namespace EasyGram.ViewModels
{
    public class ChangePasswordViewModel
    {
        [Required(ErrorMessage = "Введите электронную почту.")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "Введите пароль.")]
        [StringLength(40, MinimumLength = 8, ErrorMessage = "Пароль должен быть длиной не менее {2} символов.")]
        [DataType(DataType.Password)]
        [Display(Name = "Новый пароль")]
        [Compare("ConfirmNewPassword", ErrorMessage = "Пароли не совпадают.")]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "Подтверждение пароля обязательно.")]
        [DataType(DataType.Password)]
        [Display(Name = "Подтвердите пароль")]
        public string ConfirmNewPassword { get; set; }
    }
}
