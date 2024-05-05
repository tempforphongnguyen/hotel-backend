using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Enum
{
    public enum StatusOrderEnum
    {
        New = 0,
        Checkin = 1,
        Checkout = 2,
        Cancel = 3,
        WaitingPayment = 4,
        Delete = 6,
    }
}
