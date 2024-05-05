using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Entities
{
    public class Department : BaseEntity
    {
        public string Name { get; set; }
        public string Location { get; set; }
        public string PhoneNumber { get; set; }
        public string LocationLink { get; set; }
        public virtual ICollection<Room> Rooms { get; set; } = new List<Room>();
    }
}
