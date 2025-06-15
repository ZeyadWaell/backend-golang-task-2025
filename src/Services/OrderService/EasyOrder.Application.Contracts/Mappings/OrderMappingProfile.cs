using AutoMapper;
using EasyOrder.Application.Contracts.DTOs;
using EasyOrder.Application.Queries.DTOs;
using EasyOrder.Domain.Entities;
using EasyOrder.Domain.Enums;
using System;
using System.Linq;

namespace EasyOrder.Application.Queries.Mappings
{
    public class OrderMappingProfile : Profile
    {
        public OrderMappingProfile()
        {
            // 1) Map each CreateOrderItemDto → OrderItem
            CreateMap<CreateOrderItemDto, OrderItem>()
                // if your OrderItem has an identity or navigational props you want to ignore:
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Order, opt => opt.Ignore())
                .ForMember(dest => dest.OrderId, opt => opt.Ignore());

            // (Optional) If you also have a CreatePaymentDto:
            CreateMap<CreatePaymentDto, Payment>();

            // 2) Map the parent CreateOrderDto → Order
            CreateMap<CreateOrderDto, Order>()
                // wire up the items collection
                .ForMember(dest => dest.Items,
                           opt => opt.MapFrom(src => src.Items))
                // map your payment sub‐object (if you have one)
                .ForMember(dest => dest.Payment,
                           opt => opt.MapFrom(src => src.Payment))

                // ignore fields you’ll set yourself
                .ForMember(d => d.CreatedBy, opt => opt.Ignore())
                .ForMember(d => d.TotalAmount, opt => opt.Ignore())
                .ForMember(d => d.PlacedAt, opt => opt.Ignore())

                // after the DTO → entity mapping, populate totals, timestamps, etc.
                .AfterMap((src, dest) =>
                {
                    dest.TotalAmount = dest.Items.Sum(i => i.Quantity * i.UnitPrice);
                    dest.PlacedAt = DateTime.UtcNow;

                    if (dest.Payment is not null)
                    {
                        dest.Payment.Amount = dest.TotalAmount;
                        dest.Payment.Status = PaymentStatue.Pending;
                        dest.Payment.ProcessedAt = DateTime.UtcNow;
                    }
                });
            CreateMap<Order, OrderStatusDto>()
    .ForMember(d => d.orderStatus, opt => opt.MapFrom(src => src.Status));

            CreateMap<Order, OrderListDto>();

            // (Optional) If you still need the reverse:
            CreateMap<OrderStatusDto, Order>();

            // other maps...
            CreateMap<OrderStatusDto, Order>();
            CreateMap<Order, OrderDetailsDto>()
                .ForMember(d => d.TotalAmount,
                           opt => opt.MapFrom(src => src.TotalAmount))
                .ForMember(d => d.PaidAt,
                           opt => opt.MapFrom(src => src.Payment != null
                               ? src.Payment.ProcessedAt
                               : (DateTime?)null));
            CreateMap<OrderItem, OrderItemDto>();
            CreateMap<Payment, PaymentDto>()
                .ForMember(d => d.Currency,
                           opt => opt.MapFrom(src => src.Currency.ToString()));
        }
    }
}
