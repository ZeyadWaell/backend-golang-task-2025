using EasyOrder.Application.Command.Commands.Order;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyOrder.Application.Command.Validators.Order
{
    public class CancelOrderCommandValidator: AbstractValidator<CancelOrderCommand>
    {
        public CancelOrderCommandValidator()
        {
            RuleFor(x => x.OrderId).GreaterThan(0);
        }
    }
}
