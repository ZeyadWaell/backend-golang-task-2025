using EasyOrder.Application.Contracts.DTOs.Responses.Global;
using EasyOrder.Application.Contracts.Interfaces.Main;
using EasyOrder.Application.Contracts.Interfaces.Services;
using EasyOrder.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyOrder.Application.Contracts.Services
{

    public class PaymentService : IPaymentService
    {
        private readonly IUnitOfWork _unitOfWork;

        public PaymentService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<BaseApiResponse> ChargePaymenyAsync(PaymentMethod paymentMethod, decimal amount,int orderId)
        {
            if (paymentMethod == PaymentMethod.Cash)
            {
                var paymentSussess = true; 
                if (paymentSussess)
                {
                    var order = await _unitOfWork.OrdersRepository.GetAsync(x=>x.Id == orderId);
                    order.Status = OrderStatus.Paid; 
                    await _unitOfWork.SaveChangesAsync(); 
                }
                await Task.Delay(1000); 
                return new SuccessResponse<object>("Payment processed successfully", null);
            }
            else if (paymentMethod == PaymentMethod.Card) 
            {
                var paymentSussess = true;
                if (paymentSussess)
                {
                    var order = await _unitOfWork.OrdersRepository.GetAsync(x => x.Id == orderId);
                    order.Status = OrderStatus.Paid;
                    await _unitOfWork.SaveChangesAsync();
                }
                await Task.Delay(1000); 
                return new SuccessResponse<object>("Payment processed successfully via PayPal", null);
            }
            else
            {
                return  ErrorResponse.BadRequest("Unsupported payment method");
            }
        }

    }
}
