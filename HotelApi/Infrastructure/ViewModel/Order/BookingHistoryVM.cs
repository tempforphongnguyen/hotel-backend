using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Entities;
using Infrastructure.ViewModel.Auth;
using Infrastructure.ViewModel.OrderDetail;

namespace Infrastructure.ViewModel.Order
{
    public class BookingHistoryVM
    {
        public Entities.Order Order { get; set; }

        public OrderDetailFormatVM OrderDetail { get; set; }
        //public Review Review { get; set; }

    }
}
