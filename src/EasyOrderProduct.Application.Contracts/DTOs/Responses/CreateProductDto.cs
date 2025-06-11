using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyOrderProduct.Application.Contracts.DTOs.Responses
{
    public class CreateProductDto
    {
        [Required, MaxLength(255)]
        public string Name { get; set; }

        public string Description { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal BasePrice { get; set; }

        public IList<CreateVariationDto> Variations { get; set; }
            = new List<CreateVariationDto>();

        public IList<CreateProductItemDto> ProductItems { get; set; }
            = new List<CreateProductItemDto>();
    }

}
