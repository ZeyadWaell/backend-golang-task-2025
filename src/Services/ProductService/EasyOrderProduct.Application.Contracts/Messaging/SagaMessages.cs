using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyOrderProduct.Application.Contract.Messaging
{
    public record ReserveInventory(int OrderId, int ProductItemId, int Quantity);
    public record InventoryReserved(int OrderId);
    public record InventoryNotAvailable(int OrderId);
    public record ReleaseInventory(int OrderId, int ProductItemId, int Quantity);
}
