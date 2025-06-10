using EasyOrder.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyOrder.Application.Contracts.DTOs
{
    public class CreateOrderDto
    {

        [Required, MaxLength(3)]
        public Currency Currency { get; set; }

        /// <summary>
        /// At least one order item
        /// </summary>
        [Required, MinLength(1)]
        public List<CreateOrderItemDto> Items { get; set; }

        public CreatePaymentDto Payment { get; set; }
    }
}
