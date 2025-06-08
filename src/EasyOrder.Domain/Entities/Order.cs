using EasyOrder.Domain.Common;
using EasyOrder.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyOrder.Domain.Entities
{
    public class Order : BaseSoftIntDelete
    {      

        [Required]
        public OrderStatus Status { get; set; } = OrderStatus.Pending;

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }

        [Required, MaxLength(3)]
        public Currency Currency { get; set; }
        public DateTime PlacedAt { get; set; } = DateTime.UtcNow;
        public DateTime? PaidAt { get; set; }
        public DateTime? CancelledAt { get; set; }
        public ICollection<OrderItem> Items { get; set; }
        public Payment Payment { get; set; }
    }
}
