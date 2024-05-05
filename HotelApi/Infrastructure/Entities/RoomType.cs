using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Entities
{
    public class RoomType : BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int MaxPerson { get; set; }
        public int MaxChild { get; set; }
        public string View { get; set; }
        public double Price { get; set; }
        public virtual ICollection<ImageRoomType> ImageRoomTypes { get; set; }
        public virtual ICollection<Room> Rooms { get; set; }
        public virtual ICollection<Review> Reviews { get; set; }

    }
}
