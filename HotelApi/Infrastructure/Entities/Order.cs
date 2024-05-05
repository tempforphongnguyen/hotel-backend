using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Entities
{
    public class Order : BaseEntity
    {
        public decimal TotalPrice { get; set; }
        public string Note { get; set; }
        public double Discount { get; set; }
        public Guid UserId { get; set; }
        public virtual User User { get; set; }
        public virtual ICollection<History> Histories { get; set; }
        public Guid DepartmentId { get; set; }
        public virtual Department Department{ get; set; }
        public Guid ReviewId { get; set; }
        public virtual Review Review { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
