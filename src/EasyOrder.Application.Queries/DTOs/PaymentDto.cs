using EasyOrder.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyOrder.Application.Queries.DTOs
{
    public class PaymentDto
    {
        public string TransactionId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public PaymentMethod Method { get; set; }
        public PaymentStatue Status { get; set; }
        public DateTime? ProcessedAt { get; set; }
    }
}
