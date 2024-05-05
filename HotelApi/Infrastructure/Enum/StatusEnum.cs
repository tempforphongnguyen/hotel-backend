using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Enum
{
    public enum StatusEnum
    {
        [Description("Available")]
        Available = 0,
        [Description("NotAvailable")]
        NotAvailable = 1,
        [Description("Repair")]
        Repair = 3,
        [Description("Default")]
        Default = 4,
    }
}
