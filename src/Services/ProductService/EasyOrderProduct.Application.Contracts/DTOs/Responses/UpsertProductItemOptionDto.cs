using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyOrderProduct.Application.Contracts.DTOs.Responses
{

    public class UpsertProductItemOptionDto
    {
        public int Id { get; set; }
        [Required, MaxLength(100)]
        public string Value { get; set; }
        [Range(0, double.MaxValue)]
        public decimal PriceModifier { get; set; }
    }

}
