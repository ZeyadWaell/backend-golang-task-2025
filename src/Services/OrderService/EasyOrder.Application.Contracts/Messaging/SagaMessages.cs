using EasyOrder.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyOrder.Application.Contracts.Messaging
{
    public static class SagaMessages
    {
        public record OrderCreated(int OrderId, int ProductItemId, int Quantity, decimal TotalAmount, Currency Currency);
        public record ReserveInventory(int OrderId, int ProductItemId, int Quantity);
        public record InventoryReserved(int OrderId);
        public record InventoryNotAvailable(int OrderId);
        public record ReleaseInventory(int OrderId, int ProductItemId, int Quantity);
        public record ChargePayment(int OrderId, decimal Amount, Currency Currency);
        public record PaymentSucceeded(int OrderId);
        public record PaymentFailed(int OrderId);
        public record OrderCompleted(int OrderId);
        public record OrderCancelled(int OrderId, string Reason);
    }
}
