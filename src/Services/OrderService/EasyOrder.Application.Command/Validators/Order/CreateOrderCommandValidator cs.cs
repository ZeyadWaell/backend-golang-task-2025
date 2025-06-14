using EasyOrder.Application.Command.Commands.Order;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyOrder.Application.Command.Validators.Order
{
    public class CreateOrderCommandValidator: AbstractValidator<CreateOrderCommand>
    {
        public CreateOrderCommandValidator()
        {
            //RuleFor(x => x.OrderDto)
            //    .NotNull();

            //RuleFor(x => x.OrderDto.Currency)
            //    .IsInEnum();

            //RuleFor(x => x.OrderDto.Items)
            //    .NotEmpty().WithMessage("You must order at least one item");

            //RuleForEach(x => x.OrderDto.Items)
            //    .ChildRules(items =>
            //    {
            //        items.RuleFor(i => i.ProductItemId)
            //             .GreaterThan(0);
            //        items.RuleFor(i => i.Quantity)
            //             .GreaterThan(0);
            //    });

            //When(x => x.OrderDto.Payment != null, () =>
            //{
            //    RuleFor(x => x.OrderDto.Payment.Method)
            //        .IsInEnum();
            //});
        }
    }
}
