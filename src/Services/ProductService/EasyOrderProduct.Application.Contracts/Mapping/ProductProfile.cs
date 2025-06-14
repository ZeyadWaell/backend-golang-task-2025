using AutoMapper;
using EasyOrderProduct.Application.Contracts.DTOs.Responses;
using EasyOrderProduct.Domain.Entities;

namespace EasyOrderProduct.Application.Contracts.Mapping
{
    public class ProductProfile : Profile
    {
        public ProductProfile()
        {

            CreateMap<ProductItem, ProductItemInventoryDto>()
    .ForMember(dest => dest.ProductItemId,
               opt => opt.MapFrom(src => src.Id))
    .ForMember(dest => dest.Sku,
               opt => opt.MapFrom(src => src.Sku))
    .ForMember(dest => dest.QuantityOnHand,
               opt => opt.MapFrom(src => src.Inventory != null
                                            ? src.Inventory.QuantityOnHand
                                            : 0))
    .ForMember(dest => dest.WarehouseLocation,
               opt => opt.MapFrom(src => src.Inventory != null
                                            ? src.Inventory.WarehouseLocation
                                            : null));

            CreateMap<Product, ProductInventoryResponseDto>()
                .ForMember(dest => dest.ProductId,
                           opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name,
                           opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Items,
                           opt => opt.MapFrom(src => src.ProductItems));



            ////////////////////////////////////////////////////////////////////
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
