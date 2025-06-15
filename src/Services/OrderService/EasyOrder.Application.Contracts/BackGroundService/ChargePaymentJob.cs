using EasyOrder.Application.Contracts.Interfaces.Services;
using EasyOrder.Domain.Enums;
using Hangfire;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace EasyOrder.Application.Contracts.BackGroundService;

public class ChargePaymentJob
{
    private readonly ILogger<ChargePaymentJob> _logger;
    private readonly IPaymentService _paymentService;

    public ChargePaymentJob(ILogger<ChargePaymentJob> logger,IPaymentService paymentService)
    {
        _logger = logger;
        _paymentService = paymentService;
    }


    [AutomaticRetry(Attempts = 3, DelaysInSeconds = new[] { 60, 120, 300 }, OnAttemptsExceeded = AttemptsExceededAction.Fail)]
    public async Task ExecuteAsync(PaymentMethod paymentMethod, decimal amount, int orderId)
    {
        try
        {
            var result = await _paymentService.ChargePaymenyAsync(paymentMethod, amount, orderId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "ChargePaymentAsync failed for OrderId {OrderId} (Method={PaymentMethod}, Amount={Amount})",
                orderId, paymentMethod, amount);

            throw;
        }
    }
}
