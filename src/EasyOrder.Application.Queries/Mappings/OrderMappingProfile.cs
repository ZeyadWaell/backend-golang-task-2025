using AutoMapper;
using EasyOrder.Application.Queries.DTOs;
using EasyOrder.Domain.Entities;
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