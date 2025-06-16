using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyOrderProduct.Application.Contract.DTOs.Responses
{

        public class ProductDetailsResponseDto
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public decimal BasePrice { get; set; }
            public IList<VariationDto> Variations { get; set; } = new List<VariationDto>();
            public IList<ProductItemDto> ProductItems { get; set; } = new List<ProductItemDto>();
        }

        public class VariationDto
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public IList<VariationOptionDto> Options { get; set; } = new List<VariationOptionDto>();
        }

        public class VariationOptionDto
        {
            public int Id { get; set; }
            public string Value { get; set; }
            public decimal PriceModifier { get; set; }
        }

        public class ProductItemDto
        {
            public int Id { get; set; }
            public string Sku { get; set; }
            public decimal? PriceOverride { get; set; }
            public IList<ProductItemOptionDto> Options { get; set; } = new List<ProductItemOptionDto>();
            public InventoryDto Inventory { get; set; }
        }

        public class ProductItemOptionDto
        {
            public int VariationOptionId { get; set; }
        }

        public class InventoryDto
        {
        public int? ProductItemId { get; set; }
        public int QuantityOnHand { get; set; }
        public string WarehouseLocation { get; set; }
        }
    
}
