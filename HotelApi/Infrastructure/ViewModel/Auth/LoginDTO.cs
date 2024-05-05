using System.ComponentModel.DataAnnotations;

namespace Infrastructure.ViewModel.Auth
{
    public class LoginDTO
    {
        [Required(ErrorMessage = "ادخل البريد الالكتروني")]
        public string Email { get; set; }

        [Required(ErrorMessage = "ادخل كلمة المرور")]
        public string Password { get; set; }
    }
}
