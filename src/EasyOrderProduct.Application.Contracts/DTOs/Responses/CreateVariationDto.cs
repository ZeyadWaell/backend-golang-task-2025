using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyOrderProduct.Application.Contracts.DTOs.Responses
{
    public class CreateVariationDto
    {
        public int? Id { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; }

        public IList<CreateVariationOptionDto> Options { get; set; }
            = new List<CreateVariationOptionDto>();
    }
}
