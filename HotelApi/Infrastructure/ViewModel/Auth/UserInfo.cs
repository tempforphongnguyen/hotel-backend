using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.ViewModel.Auth
{
    public  class UserInfo
    {
        public Guid Id { get; set; }
        public string Avatar { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string FullName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Role { get; set; }
        public string Membership { get; set; }
        public decimal Merit { get; set; }
        public DateTime CreateDate { get; set; }
        public Guid RoleId { get; set; }
    }
}
