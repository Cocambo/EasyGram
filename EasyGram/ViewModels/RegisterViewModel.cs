using System.ComponentModel.DataAnnotations;

namespace EasyGram.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Введите имя и фамилию.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Введите электронную почту.")]
        [EmailAddress(ErrorMessage = "Введён некорректный адрес электронной почты.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Введите пароль.")]
        [StringLength(40, MinimumLength = 8, ErrorMessage = "Пароль должен быть длиной не менее {2} символов.")]
        [DataType(DataType.Password)]
        [Compare("ConfirmPassword", ErrorMessage = "Пароли не совпадают.")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Подтверждение пароля обязательно.")]
        [DataType(DataType.Password)]
        [Display(Name = "Подтвердите пароль.")]
        public string ConfirmPassword { get; set; }
    }
}
