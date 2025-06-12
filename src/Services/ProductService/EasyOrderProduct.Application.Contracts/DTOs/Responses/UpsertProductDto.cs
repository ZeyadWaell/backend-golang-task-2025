using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyOrderProduct.Application.Contracts.DTOs.Responses
{
    public class UpsertProductDto
    {
        public int? Id { get; set; }

        [Required, MaxLength(255)]
        public string Name { get; set; }

        public string Description { get; set; }

        [Required, Range(0, double.MaxValue)]
        public decimal BasePrice { get; set; }

        public IList<UpsertVariationDto> Variations { get; set; } = new List<UpsertVariationDto>();
        public IList<UpsertProductItemDto> ProductItems { get; set; } = new List<UpsertProductItemDto>();
    }



}
