using EasyOrder.Application.Contracts.Messaging;
using Rebus.Handlers;
using Rebus.Sagas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyOrder.Infrastructure.Sagas
{
    public class OrderSagaData : SagaData { /* fields for OrderId, etc. */ }

    public class OrderSaga :
        Saga<OrderSagaData>,
        IAmInitiatedBy<SagaMessages.OrderCreated>,
        IHandleMessages<SagaMessages.InventoryReserved>,
        IHandleMessages<SagaMessages.InventoryNotAvailable>,
        IHandleMessages<SagaMessages.PaymentSucceeded>,
        IHandleMessages<SagaMessages.PaymentFailed>
    {
        // correlation config as shown earlier

        public async Task Handle(SagaMessages.OrderCreated msg) { /* send ReserveInventory */ }
        public async Task Handle(SagaMessages.InventoryReserved msg) { /* send ChargePayment */ }
        public async Task Handle(SagaMessages.InventoryNotAvailable msg) { /* publish OrderCancelled; complete */ }
        public async Task Handle(SagaMessages.PaymentSucceeded msg) { /* publish OrderCompleted; complete */ }
        public async Task Handle(SagaMessages.PaymentFailed msg) { /* send ReleaseInventory; publish OrderCancelled; complete */ }

        protected override void CorrelateMessages(ICorrelationConfig<OrderSagaData> config)
        {
            throw new NotImplementedException();
        }
    }
}
