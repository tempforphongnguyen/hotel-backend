using B2Net.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.IService
{
    public interface IUploadFileService
    {
        Task<string> UploadFileAvatar(IFormFile file);
        Task<string> UploadImage(IFormFile file);
    }
}
