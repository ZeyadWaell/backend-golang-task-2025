using EasyOrder.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyOrder.Application.Contracts.DTOs
{
    public class CreatePaymentDto
    {
        [Required]
        public PaymentMethod Method { get; set; }
    }
}
