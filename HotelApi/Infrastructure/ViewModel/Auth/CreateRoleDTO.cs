using System.ComponentModel.DataAnnotations;

namespace Infrastructure.ViewModel.Auth
{
    public class CreateRoleDTO
    {
        [Required(ErrorMessage = "ادخل الصلاحية")]
        public string RoleName { get; set; }
    }
}
