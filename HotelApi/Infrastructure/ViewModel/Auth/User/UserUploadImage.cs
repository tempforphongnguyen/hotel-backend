using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.ViewModel.Auth.User
{
    public class UserUploadImage
    {
        public IFormFile File { get; set; }
        public Guid Id { get; set; }
    }
}
