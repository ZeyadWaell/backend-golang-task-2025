using EasyOrder.Application.Contracts.DTOs;
using EasyOrder.Application.Contracts.Interfaces.Services;
using EasyOrder.Domain.Enums;
using Hangfire;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyOrder.Application.Contracts.BackGroundService
{
    public class CreatingOrderWaterPoolJob
    {
        private readonly ILogger<ChargePaymentJob> _logger;
        private readonly IOrderService _order;

        public CreatingOrderWaterPoolJob(ILogger<ChargePaymentJob> logger,IOrderService order)
        {
            _logger = logger;
            _order = order;
        }


        [AutomaticRetry(Attempts = 1, DelaysInSeconds = new[] { 60, 120, 300 }, OnAttemptsExceeded = AttemptsExceededAction.Fail)]
        public async Task ExecuteAsync(CreateOrderDto dto)
        {
            try
            {
                var result = await _order.CreateOrderAsync(dto);
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex,
                //    "ChargePaymentAsync failed for OrderId {OrderId} (Method={PaymentMethod}, Amount={Amount})",
                //    orderId, paymentMethod, amount);

                throw;
            }
        }
    }
}
