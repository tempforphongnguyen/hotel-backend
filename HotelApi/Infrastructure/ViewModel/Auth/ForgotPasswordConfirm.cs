using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.ViewModel.Auth
{
    public class ForgotPasswordConfirm
    {
        public string UserEmail { get; set; }
        public string Password { get; set; }
        public int ForgotPasswordCode { get; set; }
    }
}
