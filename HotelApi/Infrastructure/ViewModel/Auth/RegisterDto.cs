using System.ComponentModel.DataAnnotations;

namespace Infrastructure.ViewModel.Auth
{
    public class RegisterDto
    {
        [StringLength(100)]
        public string FirstName { get; set; }

        [StringLength(100)]
        public string LastName { get; set; }

        [StringLength(128)]
        public string Email { get; set; }

        [StringLength(256)]
        public string Password { get; set; }

        public DateTime DateOfBirth { get; set; }
        public string PhoneNumber { get; set; }
    }
}
