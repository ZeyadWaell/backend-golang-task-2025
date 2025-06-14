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
    public class Payment : BaseSoftDelete
    {
        [Key, ForeignKey(nameof(Order))]
        public int OrderId { get; set; }

        [Required, MaxLength(200)]
        public Guid TransactionId { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [Required, MaxLength(3)]
        public Currency Currency { get; set; }

        [Required, MaxLength(50)]
        public PaymentMethod Method { get; set; }    

        [Required, MaxLength(50)]
        public PaymentStatue Status { get; set; }    

        public DateTime? ProcessedAt { get; set; }
        public Order Order { get; set; }
    }
}
