using EasyOrderProduct.Application.Contracts.DTOs.Responses;
using EasyOrderProduct.Application.Contracts.DTOs.Responses.Global;
using EasyOrderProduct.Application.Contracts.Interfaces;
using EasyOrderProduct.Application.Contracts.Interfaces.Main;
using EasyOrderProduct.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyOrderProduct.Application.Contracts.Services
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        public ProductService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<BaseApiResponse> CreateProductAsync(CreateProductDto dto)
        {
            var product = new Product
            {
                Name = dto.Name,
                Description = dto.Description,
                BasePrice = dto.BasePrice
            };

            MapVariations(product, dto.Variations);
            MapProductItems(product, dto.ProductItems);

            await _unitOfWork.ProductRepository.AddAsync(product);
            await _unitOfWork.SaveChangesAsync();
            return new SuccessResponse<object>("Product Id", product.Id, 200);

        }

        #region Private Helpers
        private void MapVariations(Product product, IList<CreateVariationDto> dtos)
        {
            foreach (var dto in dtos)
            {
                var variation = dto.Id.HasValue
                    ? product.Variations.FirstOrDefault(v => v.Id == dto.Id.Value)
                    : null;

                if (variation == null)
                {
                    variation = new Variation();
                    product.Variations.Add(variation);
                }

                variation.Name = dto.Name;
                MapOptions(variation, dto.Options);
            }
        }

        private void MapOptions(Variation variation, IList<CreateVariationOptionDto> dtos)
        {
            foreach (var dto in dtos)
            {
                var option = dto.Id.HasValue
                    ? variation.Options.FirstOrDefault(o => o.Id == dto.Id.Value)
                    : null;

                if (option == null)
                {
                    option = new VariationOption();
                    variation.Options.Add(option);
                }

                option.Value = dto.Value;
                option.PriceModifier = dto.PriceModifier;
            }
        }

        private void MapProductItems(Product product, IList<CreateProductItemDto> dtos)
        {
            foreach (var dto in dtos)
            {
                var item = new ProductItem
                {
                    Sku = dto.Sku,
                    PriceOverride = dto.PriceOverride
                };

                foreach (var optId in dto.VariationOptionIds)
                {
                    item.Options.Add(new ProductItemOption
                    {
                        VariationOptionId = optId
                    });
                }

                item.Inventory = new Inventory
                {
                    QuantityOnHand = dto.QuantityOnHand,
                    WarehouseLocation = dto.WarehouseLocation
                };

                product.ProductItems.Add(item);
            }
        }
        #endregion

    }
}
