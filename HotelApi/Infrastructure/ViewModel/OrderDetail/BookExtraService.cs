﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.ViewModel.OrderDetail
{
    public class BookExtraService
    {
        public Guid OrderId { get; set; } = Guid.Empty;
        public Guid ExtraServiceId { get; set; }
        public int Quantity { get; set; }
        public int UnitPrice { get; set; }
        public string ExtraServiceName { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
