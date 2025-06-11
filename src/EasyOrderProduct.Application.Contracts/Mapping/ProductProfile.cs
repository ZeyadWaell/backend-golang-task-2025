using AutoMapper;
using EasyOrderProduct.Application.Contracts.DTOs.Responses;
using EasyOrderProduct.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace EasyOrderProduct.Application.Contracts.Mapping
{
    public class ProductProfile : Profile
    {
        public ProductProfile()
        {
            CreateMap<VariationOption, VariationOptionDto>();
            CreateMap<Variation, VariationDto>();
            CreateMap<ProductItemOption, ProductItemOptionDto>()
                .ForMember(dest => dest.VariationOptionId,opt => opt.MapFrom(src => src.VariationOption.Id));
            CreateMap<ProductItem, ProductItemDto>();
            CreateMap<Inventory, InventoryDto>();
            CreateMap<Product, ProductResponseDto>();
        }
    }
}
