using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DATS
{
    public class LoginViewModel
    {
        [DisplayName("Имя пользователя")]
        [Required(ErrorMessage = "Пожалуйста введите имя пользователя.")]
        public string UserName { get; set; }

        [DisplayName("Пароль")]
        [Required(ErrorMessage = "Пожалуйста введите пароль.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}