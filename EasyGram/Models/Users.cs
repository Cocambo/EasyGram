using Microsoft.AspNetCore.Identity;

namespace EasyGram.Models
{
    public class Users : IdentityUser //  Наследуем от IdentityUser
    {
        public string FullName { get; set; } // Т.к в классе родителе нет свойства FullName, добавим его сами
    }
}
