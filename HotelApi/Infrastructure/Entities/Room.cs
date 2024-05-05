using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Entities
{
    public class Room : BaseEntity
    {
        public string Name { get; set; }
        public Guid DepartmentId { get; set; }
        public virtual Department Department { get; set; }
        public Guid RoomTypeId { get; set; }
        public virtual RoomType RoomType { get; set; }
    }
}
