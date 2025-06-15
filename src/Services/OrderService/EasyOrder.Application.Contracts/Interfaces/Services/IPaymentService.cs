using EasyOrder.Application.Contracts.DTOs.Responses.Global;
using EasyOrder.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyOrder.Application.Contracts.Interfaces.Services
{
    public interface IPaymentService
    {
        Task<BaseApiResponse> ChargePaymenyAsync(PaymentMethod paymentMethod, decimal amount, int orderId);
    }
}
