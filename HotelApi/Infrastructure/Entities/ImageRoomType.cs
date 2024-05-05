using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Entities
{
    public class ImageRoomType
    {
        public string FileName { get; set; }
        public Guid RoomTypeId { get; set; }
        public virtual RoomType RoomType { get; set; }
    }   
}
