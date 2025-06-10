using AutoMapper;
using EasyOrder.Application.Contracts.DTOs;
using EasyOrder.Application.Queries.DTOs;
using EasyOrder.Domain.Entities;
using EasyOrder.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace EasyOrder.Application.Queries.Mappings
{
    public class OrderMappingProfile : Profile
    {
        public OrderMappingProfile()
        {
            CreateMap<CreateOrderDto, Order>()
                .ForMember(d => d.CreatedBy, opt => opt.Ignore())
                .ForMember(d => d.TotalAmount, opt => opt.Ignore())
                .ForMember(d => d.PlacedAt, opt => opt.Ignore())
                .ForMember(d => d.Payment, opt => opt.MapFrom(src => src.Payment))
                .AfterMap((src, dest) =>
                {
                    dest.TotalAmount = dest.Items.Sum(i => i.Quantity * i.UnitPrice);
                    dest.PlacedAt = DateTime.UtcNow;
                    if (dest.Payment != null)
                    {
                        dest.Payment.Amount = dest.TotalAmount;
                        dest.Payment.Status = PaymentStatue.Pending;
                        dest.Payment.ProcessedAt = DateTime.UtcNow;
                    }
                });


            CreateMap<Order, OrderDetailsDto>()
                        .ForMember(dest => dest.TotalAmount,
                                   opt => opt.MapFrom(src => src.TotalAmount))
                        .ForMember(dest => dest.PaidAt,
                                   opt => opt.MapFrom(src => src.Payment != null
                                       ? src.Payment.ProcessedAt
                                       : (DateTime?)null));

            CreateMap<OrderItem, OrderItemDto>();

            CreateMap<Payment, PaymentDto>()
                .ForMember(dest => dest.Currency,
                           opt => opt.MapFrom(src => src.Currency.ToString()));
        }
    }
}