using System.ComponentModel.DataAnnotations;

namespace EasyGram.ViewModels
{
    public class VerifyEmailViewModel
    {
        [Required(ErrorMessage = "Введите электронную почту.")]
        [EmailAddress(ErrorMessage = "Введён некорректный адрес электронной почты.")]
        public string Email { get; set; }
    }
}
