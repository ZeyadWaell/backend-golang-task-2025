using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyOrderProduct.Application.Contract.DTOs.Responses
{
    public class LowStockItemDto
    {
        public int ProductItemId { get; set; }
        public string Sku { get; set; }
        public int QuantityOnHand { get; set; }
        public string WarehouseLocation { get; set; }
    }
}
