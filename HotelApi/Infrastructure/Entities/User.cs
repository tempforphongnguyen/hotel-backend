using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Entities
{
    public class User : IdentityUser<Guid>
    {
        [StringLength(100)]
        public string FullName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime ExpRefreshToken { get; set; }
        public string Avatar { get; set; }
        public int ForgotPasswordCode { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Membership { get; set; }
        public decimal Merit { get; set; }
        public DateTime CreateDate { get; set; }
    }

}
