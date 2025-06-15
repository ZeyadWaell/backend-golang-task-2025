using AutoMapper;
using EasyOrderProduct.Application.Contract.DTOs.Responses;
using EasyOrderProduct.Application.Contract.Interfaces.Services;
using EasyOrderProduct.Application.Contracts.DTOs.Responses;
using EasyOrderProduct.Application.Contracts.DTOs.Responses.Global;
using EasyOrderProduct.Application.Contracts.Filters;
using EasyOrderProduct.Application.Contracts.Interfaces.InternalServices;
using EasyOrderProduct.Application.Contracts.Interfaces.Main;
using EasyOrderProduct.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace EasyOrderProduct.Application.Contracts.Services
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUserService;
        public ProductService(IUnitOfWork unitOfWork, IMapper mapper, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _currentUserService = currentUserService;
        }
        public async Task<BaseApiResponse> CreateProductAsync(UpsertProductDto dto)
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
            return new SuccessResponse<object>("Added Fully", product.Id, 200);
        }
        public async Task<BaseApiResponse> GetByIdAsync(int id)
        {
            var product = await _unitOfWork.ProductRepository.GetIncludingAsync(p => p.Id == id,x=>x.Variations,x=>x.ProductItems);

            if (product == null)
                return ErrorResponse.NotFound("Product was not found");

            var mapping =  _mapper.Map<ProductDetailsResponseDto>(product);

            return new SuccessResponse<object>("Product found", mapping, 200);
        }

        public async Task<BaseApiResponse> GetAllAsync(PaginationFilter filter)
        {
            var products = await _unitOfWork.ProductRepository.GetAllPaginatedAsync(x => x.CreatedBy == _currentUserService.UserId, filter);

            var mapping = _mapper.Map<IList<ProductResponseDto>>(products);

            if (products == null || !products.Any())
                return new SuccessResponse<object>("Products found", mapping, 200);


            return new SuccessResponse<object>("Products found", mapping, 200);
        }
        public async Task<BaseApiResponse> UpsertAsync(UpsertProductDto dto)
        {
            Product product;
            #region Handle Id
            if (dto.Id.HasValue)
            {
                product = await _unitOfWork.ProductRepository
                                  .GetAsync(p => p.Id == dto.Id.Value);
                if (product == null)
                    return ErrorResponse.NotFound("Product not found");
            }
            else
            {
                product = new Product();
            }
            #endregion
            product.Name = dto.Name;
            product.Description = dto.Description;
            product.BasePrice = dto.BasePrice;

            UpdateVariations(product, dto.Variations);
            UpdateProductItems(product, dto.ProductItems);

            if (!dto.Id.HasValue)
                await _unitOfWork.ProductRepository.AddAsync(product);
            _unitOfWork.ProductRepository.Update(product);
            await _unitOfWork.SaveChangesAsync();

            return new SuccessResponse<int>(
                message: "Product saved successfully",
                data: product.Id,
                 200
            );
        }

        public async Task<BaseApiResponse> GetInventoryAsync(int productId)
        {
            var product = await _unitOfWork.ProductRepository.GetWithItemsAndInventoryAsync(productId);

            if (product == null)
                return ErrorResponse.NotFound("Product not found");

            var result = _mapper.Map<ProductInventoryResponseDto>(product);

            return new SuccessResponse<object>(
                message: "Product inventory retrieved successfully",
                data: result,
                statusCode: 200
            );
        }
        #region UpdateProduct
        private void UpdateVariations(Product product,
                                      IList<UpsertVariationDto> dtos)
        {
            if (dtos == null)
            {
                product.Variations.Clear();
                return;
            }

            // remove any not in DTO
            var toRemove = product.Variations
                .Where(v => !dtos.Any(d => d.Id == v.Id))
                .ToList();
            toRemove.ForEach(v => product.Variations.Remove(v));

            // upsert each DTO
            foreach (var d in dtos)
            {
                var entity = d.Id.HasValue
                    ? product.Variations.FirstOrDefault(v => v.Id == d.Id.Value)
                    : null;

                if (entity == null)
                {
                    entity = new Variation();
                    product.Variations.Add(entity);
                }

                entity.Name = d.Name;
            }
        }

        private void UpdateProductItems(Product product,
                                        IList<UpsertProductItemDto> dtos)
        {
            if (dtos == null)
            {
                product.ProductItems.Clear();
                return;
            }

            // remove any item not in DTO
            var toRemove = product.ProductItems
                .Where(pi => !dtos.Any(d => d.Id == pi.Id))
                .ToList();
            toRemove.ForEach(pi => product.ProductItems.Remove(pi));

            // upsert each item
            foreach (var d in dtos)
            {
                var item = d.Id.HasValue
                    ? product.ProductItems.FirstOrDefault(pi => pi.Id == d.Id.Value)
                    : null;

                if (item == null)
                {
                    item = new ProductItem();
                    product.ProductItems.Add(item);
                }

                item.Sku = d.Sku;
                item.PriceOverride = d.PriceOverride;

                // options & variation-options
                UpdateItemOptions(product, item, d.Options);

                // inventory
                if (item.Inventory == null)
                    item.Inventory = new Inventory();
                item.Inventory.QuantityOnHand = d.QuantityOnHand;
                item.Inventory.WarehouseLocation = d.WarehouseLocation;
            }
        }

        private void UpdateItemOptions(Product product,
                                       ProductItem item,
                                       IList<UpsertProductItemOptionDto> dtos)
        {
            if (dtos == null)
            {
                item.Options.Clear();
                return;
            }

            // remove links not in DTO
            var toRemove = item.Options
                .Where(opt => !dtos.Any(d => d.Id == opt.VariationOption.Id))
                .ToList();
            toRemove.ForEach(opt => item.Options.Remove(opt));

            // upsert each option
            foreach (var d in dtos)
            {
                var link = item.Options
                    .FirstOrDefault(opt => opt.VariationOption.Id == d.Id);

                if (link == null)
                {
                    // find existing VariationOption or create under first Variation
                    var vo = product.Variations
                              .SelectMany(v => v.Options)
                              .FirstOrDefault(o => o.Id == d.Id);

                    if (vo == null)
                    {
                        var firstVar = product.Variations.FirstOrDefault()
                                       ?? throw new ArgumentException("No variation to attach option");
                        vo = new VariationOption
                        {
                            Value = d.Value,
                            PriceModifier = d.PriceModifier
                        };
                        firstVar.Options.Add(vo);
                    }
                    else
                    {
                        // if already exists, update its fields
                        vo.Value = d.Value;
                        vo.PriceModifier = d.PriceModifier;
                    }

                    link = new ProductItemOption
                    {
                        VariationOption = vo
                    };
                    item.Options.Add(link);
                }
                else
                {
                    // update existing variation‐option fields
                    link.VariationOption.Value = d.Value;
                    link.VariationOption.PriceModifier = d.PriceModifier;
                }
            }
        }
        #endregion

        #region Helper Methods
        private void MapVariations(Product product, IList<UpsertVariationDto> dtos)
        {
            foreach (var dto in dtos)
            {
                var varEntity = dto.Id.HasValue
                    ? product.Variations.FirstOrDefault(v => v.Id == dto.Id.Value)
                    : null;

                if (varEntity == null)
                {
                    varEntity = new Variation();
                    product.Variations.Add(varEntity);
                }

                varEntity.Name = dto.Name;
            }
        }

        private void MapProductItems(Product product, IList<UpsertProductItemDto> dtos)
        {
            var defaultVariation = product.Variations.FirstOrDefault();

            foreach (var dto in dtos)
            {
                var item = new ProductItem
                {
                    Sku = dto.Sku,
                    PriceOverride = dto.PriceOverride
                };

                foreach (var opt in dto.Options)
                {
                    VariationOption optionEntity = null;
                    if (opt.Id > 0)
                    {
                        optionEntity = product.Variations
                            .SelectMany(v => v.Options)
                            .FirstOrDefault(o => o.Id == opt.Id);
                    }

                    if (optionEntity == null && defaultVariation != null)
                    {
                        optionEntity = new VariationOption
                        {
                            Value = opt.Value,
                            PriceModifier = opt.PriceModifier
                        };
                        defaultVariation.Options.Add(optionEntity);
                    }

                    if (optionEntity == null)
                        throw new ArgumentException($"Invalid option id: {opt.Id}");

                    item.Options.Add(new ProductItemOption
                    {
                        VariationOption = optionEntity
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
