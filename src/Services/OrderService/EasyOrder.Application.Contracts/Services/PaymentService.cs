using EasyOrder.Application.Contracts.DTOs;
using EasyOrder.Application.Contracts.DTOs.Responses.Global;
using EasyOrder.Application.Contracts.Hubs;
using EasyOrder.Application.Contracts.Interfaces.Main;
using EasyOrder.Application.Contracts.Interfaces.Services;
using EasyOrder.Domain.Enums;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace EasyOrder.Application.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<PaymentService> _logger;
        private readonly IHubContext<OrderStatusHub> _hub;

        public PaymentService(IUnitOfWork unitOfWork,ILogger<PaymentService> logger,IHubContext<OrderStatusHub> hub)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _hub = hub;
        }

        public async Task<BaseApiResponse> ChargePaymenyAsync(PaymentMethod paymentMethod, decimal amount, int orderId)
        {
            try
            {
                await Task.Delay(1000);
                var paymentSuccess = true;

                if (!paymentSuccess)
                    return ErrorResponse.BadRequest("Payment gateway rejected the transaction");

                var order = await _unitOfWork.OrdersRepository.GetAsync(x => x.Id == orderId);
                if (order == null)
                {
                    _logger.LogWarning("Order {OrderId} not found in ChargePaymenyAsync", orderId);
                    return ErrorResponse.BadRequest($"Order with ID {orderId} does not exist");
                }

                order.Status = OrderStatus.Paid;
                _unitOfWork.OrdersRepository.Update(order);

                await _unitOfWork.SaveChangesAsync();

                await _hub.Clients.Group($"order_{orderId}")
                    .SendAsync("OrderStatusUpdated", new OrderStatusSignalDto { OrderId = order.Id, Status = order.Status.ToString() });

                var msg = paymentMethod == PaymentMethod.Cash
                    ? "Payment processed successfully"
                    : "Payment processed successfully via Card";
                return new SuccessResponse<object>(msg, null);
            }
            catch (Exception ex)
            {
                // Log the full exception
                _logger.LogError(ex,
                    "Error in ChargePaymenyAsync for OrderId {OrderId} (Method={PaymentMethod}, Amount={Amount})",
                    orderId, paymentMethod, amount);

                // Return a generic error response with the exception message
                return  ErrorResponse.BadRequest(
                    "An unexpected error occurred while processing payment",
                    ex.Message.ToString());
            }
        }
    }
}
