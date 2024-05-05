using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.ViewModel.RoomType
{
    public class UpdateImageRoomType
    {
        public IFormFile File { get; set; }
        public Guid RoomTypeId { get; set; }
        public string FileName { get; set; }
    }
}
