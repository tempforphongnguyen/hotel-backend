using System.ComponentModel.DataAnnotations;

namespace Infrastructure.ViewModel.Auth
{
    public class AddRoleToUserDto
    {
        public Guid UserId { get; set; }

        public Guid RoleId { get; set; }
    }
}
