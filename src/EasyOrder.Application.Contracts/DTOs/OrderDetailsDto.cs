using EasyOrder.Domain.Common;
using EasyOrder.Domain.Entities;
using EasyOrder.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EasyOrder.Application.Queries.DTOs;

namespace EasyOrder.Application.Contracts.DTOs
{
    public class OrderDetailsDto
    {
        public int Id { get; set; }
        public OrderStatus Status { get; set; }
        public decimal TotalAmount { get; set; }
        public Currency Currency { get; set; }
        public DateTime PlacedAt { get; set; }
        public DateTime? PaidAt { get; set; }
        public DateTime? CancelledAt { get; set; }
        public ICollection<OrderItemDto> Items { get; set; }
        public PaymentDto Payment { get; set; }
    }
}