using AutoMapper;
using EasyOrderProduct.Application.Contract.DTOs.Responses;
using EasyOrderProduct.Application.Contracts.DTOs.Responses;
using EasyOrderProduct.Domain.Entities;

namespace EasyOrderProduct.Application.Contracts.Mapping
{
    public class ProductProfile : Profile
    {
        public ProductProfile()
        {

            // 1) Lightweight list‐item mapping
            CreateMap<Product, ProductResponseDto>()
                .ForMember(d => d.Id, opt => opt.MapFrom(s => s.Id))
                .ForMember(d => d.Name, opt => opt.MapFrom(s => s.Name))
                .ForMember(d => d.Description, opt => opt.MapFrom(s => s.Description))
                .ForMember(d => d.BasePrice, opt => opt.MapFrom(s => s.BasePrice));

            // 2) Full details mapping
            CreateMap<Product, ProductDetailsResponseDto>()
                .ForMember(d => d.Id, opt => opt.MapFrom(s => s.Id))
                .ForMember(d => d.Name, opt => opt.MapFrom(s => s.Name))
                .ForMember(d => d.Description, opt => opt.MapFrom(s => s.Description))
                .ForMember(d => d.BasePrice, opt => opt.MapFrom(s => s.BasePrice))
                .ForMember(d => d.Variations, opt => opt.MapFrom(s => s.Variations))
                .ForMember(d => d.ProductItems, opt => opt.MapFrom(s => s.ProductItems));

            // Nested types for details:
            CreateMap<Variation, VariationDto>();
            CreateMap<VariationOption, VariationOptionDto>();
            CreateMap<ProductItem, ProductItemDto>();
            CreateMap<ProductItemOption, ProductItemOptionDto>();
            CreateMap<Inventory, InventoryDto>();


            // 1) Map Product → ProductInventoryResponseDto
            CreateMap<Product, ProductInventoryResponseDto>()
                .ForMember(d => d.ProductId,
                           opt => opt.MapFrom(s => s.Id))
                .ForMember(d => d.Name,
                           opt => opt.MapFrom(s => s.Name))
                .ForMember(d => d.Items,
                           opt => opt.MapFrom(s => s.ProductItems));

            // 2) Map ProductItem → ProductItemInventoryDto
            CreateMap<ProductItem, ProductItemInventoryDto>()
                .ForMember(d => d.ProductItemId,
                           opt => opt.MapFrom(s => s.Id))
                .ForMember(d => d.Sku,
                           opt => opt.MapFrom(s => s.Sku))
                .ForMember(d => d.QuantityOnHand,
                           opt => opt.MapFrom(s => s.Inventory != null
                                                        ? s.Inventory.QuantityOnHand
                                                        : 0))
                .ForMember(d => d.WarehouseLocation,
                           opt => opt.MapFrom(s => s.Inventory != null
                                                        ? s.Inventory.WarehouseLocation
                                                        : null));


        }
    }
}
