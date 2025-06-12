using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyOrderProduct.Application.Contracts.DTOs.Responses
{
    public class ProductInventoryResponseDto
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public IList<ProductItemInventoryDto> Items { get; set; }
            = new List<ProductItemInventoryDto>();
    }

}
