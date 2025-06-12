using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyOrderProduct.Application.Contracts.DTOs.Responses
{
    public class ProductItemInventoryDto
    {
        public int ProductItemId { get; set; }
        public string Sku { get; set; }
        public int QuantityOnHand { get; set; }
        public string WarehouseLocation { get; set; }
    }
}
