using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyOrder.Application.Contracts.DTOs
{
    public class OrderStatusSignalDto
    {
        public int OrderId { get; set; }
        public string Status { get; set; }
    }
}
