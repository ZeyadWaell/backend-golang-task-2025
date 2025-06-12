using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyOrderProduct.Application.Contracts.DTOs.Responses
{
    public class UpsertProductItemDto
    {
        public int? Id { get; set; }

        [Required, MaxLength(100)]
        public string Sku { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? PriceOverride { get; set; }

        [Required]
        public IList<UpsertProductItemOptionDto> Options { get; set; } = new List<UpsertProductItemOptionDto>();

        [Range(0, int.MaxValue)]
        public int QuantityOnHand { get; set; }

        [MaxLength(100)]
        public string WarehouseLocation { get; set; }
    }

}
