using EasyOrderProduct.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyOrderProduct.Domain.Entities;

public class Inventory : BaseSoftIntDelete
{
    public int ProductItemId { get; set; }
    public ProductItem ProductItem { get; set; }
    public int QuantityOnHand { get; set; }
    [MaxLength(100)]
    public string WarehouseLocation { get; set; }
    [Timestamp] public byte[] RowVersion { get; set; }

}
