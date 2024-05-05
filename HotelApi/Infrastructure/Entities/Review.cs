using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Entities
{
    public class Review : BaseEntity
    {
        public int Star { get; set; }
        public string Description { get; set; }
        public Guid UserId { get; set; }
        public virtual User User { get; set; }
        public Guid RoomTypeId { get; set; }
        public virtual RoomType RoomType { get; set; }
        public Guid OrderId { get; set; }
        public virtual Order Order { get; set; }
    }
}
