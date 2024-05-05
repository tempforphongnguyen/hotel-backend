using Infrastructure.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Entities
{
    public abstract class BaseEntity
    {
        public BaseEntity()
        {
            Id = Guid.NewGuid();
            CreateDate = DateTime.UtcNow;
            Status = StatusEnum.Default.ToString();
            IsActive = true;
        }
        [Key]
        public Guid Id { get; set; }
        public DateTime CreateDate { get; set; }
        public string Status { get; set; }
        public bool IsActive { get; set; }
        public DateTime UpdateDate { get; set; }
    }
}
