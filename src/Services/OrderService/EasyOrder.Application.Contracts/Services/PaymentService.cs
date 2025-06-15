using EasyOrder.Application.Contracts.DTOs.Responses.Global;
using EasyOrder.Application.Contracts.Interfaces.Main;
using EasyOrder.Application.Contracts.Interfaces.Services;
using EasyOrder.Domain.Enums;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace EasyOrder.Application.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<PaymentService> _logger;

        public PaymentService(
            IUnitOfWork unitOfWork,
            ILogger<PaymentService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<BaseApiResponse> ChargePaymenyAsync(PaymentMethod paymentMethod, decimal amount, int orderId)
        {
            try
            {
                // Simulate external payment processing
                await Task.Delay(1000);
                var paymentSuccess = true;

                if (!paymentSuccess)
                    return ErrorResponse.BadRequest("Payment gateway rejected the transaction");

                // 1) Fetch the order
                var order = await _unitOfWork.OrdersRepository.GetAsync(x => x.Id == orderId);
                if (order == null)
                {
                    _logger.LogWarning("Order {OrderId} not found in ChargePaymenyAsync", orderId);
                    return ErrorResponse.BadRequest($"Order with ID {orderId} does not exist");
                }

                // 2) Update its status
                order.Status = OrderStatus.Paid;
                _unitOfWork.OrdersRepository.Update(order);

                // 3) Save changes
                await _unitOfWork.SaveChangesAsync();

                // 4) Return success
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
