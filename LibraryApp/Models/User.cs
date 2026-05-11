using System.ComponentModel.DataAnnotations;

namespace LibraryApp.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Введите логин")]
        public string Login { get; set; } = string.Empty;

        [Required(ErrorMessage = "Введите пароль")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        public string Role { get; set; } = "Client"; // Admin или Client
    }
}