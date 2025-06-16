using AutoMapper;
using EasyOrderProduct.Application.Contract.DTOs.Responses;
using EasyOrderProduct.Application.Contract.Interfaces.Services;
using EasyOrderProduct.Application.Contracts.DTOs.Responses;
using EasyOrderProduct.Application.Contracts.DTOs.Responses.Global;
using EasyOrderProduct.Application.Contracts.Filters;
using EasyOrderProduct.Application.Contracts.Interfaces.InternalServices;
using EasyOrderProduct.Application.Contracts.Interfaces.Main;
using EasyOrderProduct.Domain.Entities;
using Microsoft.Extensions.Caching.Memory;

public class ProductService : IProductService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMemoryCache _cache;
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(10);

    public ProductService(IUnitOfWork unitOfWork,IMapper mapper,ICurrentUserService currentUserService,IMemoryCache cache)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _currentUserService = currentUserService;
        _cache = cache;
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

        EvictPage(1, 10);

        return new SuccessResponse<object>(
             message: $"Product '{product.Name}' (ID={product.Id}) created successfully.",
             data: product.Id,
             statusCode: 201
         );
    }

    public async Task<BaseApiResponse> GetByIdAsync(int id)
    {
        var product = await _unitOfWork.ProductRepository
            .GetIncludingAsync(p => p.Id == id, p => p.Variations, p => p.ProductItems);
        if (product == null)
            return ErrorResponse.NotFound("Product was not found");

        var dto = _mapper.Map<ProductDetailsResponseDto>(product);
        return new SuccessResponse<object>(
            message: $"Details for product '{product.Name}' (ID={id}) retrieved.",
            data: dto,
            statusCode: 200
        );
    }

    public async Task<BaseApiResponse> GetAllAsync(PaginationFilter filter)
    {
        var key = CacheKey(filter.PageNumber, filter.PageSize);

        if (!_cache.TryGetValue(key, out PagedList<ProductResponseDto> dtoPage))
        {
            var paged = await _unitOfWork.ProductRepository.GetAllPaginatedAsync(p => p.CreatedBy == _currentUserService.UserId, filter);

            var list = paged.Items.Select(p => _mapper.Map<ProductResponseDto>(p)).ToList();

            dtoPage = new PagedList<ProductResponseDto>(list, paged.TotalCount, filter.PageNumber, filter.PageSize);

            _cache.Set(key, dtoPage, CacheDuration);
        }

        return new SuccessResponse<object>(
            message: $"Page {filter.PageNumber} of products retrieved. ({dtoPage.Items.Count} items)",
            data: dtoPage,
            statusCode: 200
        );
    }

    public async Task<BaseApiResponse> UpsertAsync(UpsertProductDto dto)
    {
        Product product;
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

        product.Name = dto.Name;
        product.Description = dto.Description;
        product.BasePrice = dto.BasePrice;

        UpdateVariations(product, dto.Variations);
        UpdateProductItems(product, dto.ProductItems);

        if (!dto.Id.HasValue)
            await _unitOfWork.ProductRepository.AddAsync(product);
        _unitOfWork.ProductRepository.Update(product);

        await _unitOfWork.SaveChangesAsync();

        await EvictPageContainingProductAsync(product.Id);

        return new SuccessResponse<int>(
            message: $"Product '{product.Name}' (ID={product.Id}) updated successfully.",
            data: product.Id,
            statusCode: dto.Id.HasValue ? 200 : 201
        );
    }

    public async Task<BaseApiResponse> GetInventoryAsync(int productId)
    {
        var product = await _unitOfWork.ProductRepository
            .GetWithItemsAndInventoryAsync(productId);
        if (product == null)
            return ErrorResponse.NotFound("Product not found");

        var result = _mapper.Map<ProductInventoryResponseDto>(product);
        return new SuccessResponse<object>("Inventory retrieved", result, 200);
    }

    #region Cache eviction

    private string CacheKey(int page, int size)
        => $"_Products_P{page}_S{size}";

    private void EvictPage(int page, int size)
    {
        _cache.Remove(CacheKey(page, size));
    }

    private async Task EvictPageContainingProductAsync(int productId)
    {
        var product = await _unitOfWork.ProductRepository.GetAsync(p => p.Id == productId);
        var beforeCount = await _unitOfWork.ProductRepository.CountAsync(p =>p.CreatedBy == _currentUserService.UserId &&p.CreatedOn > product.CreatedOn);

        int pageSize = 10; 
        int pageNumber = (beforeCount / pageSize) + 1;

        EvictPage(pageNumber, pageSize);
    }

    #endregion

    #region Update / Map Helpers (exactly as your logic)

    private void UpdateVariations(Product product, IList<UpsertVariationDto> dtos)
    {
        if (dtos == null) { product.Variations.Clear(); return; }
        var toRemove = product.Variations
            .Where(v => !dtos.Any(d => d.Id == v.Id))
            .ToList();
        toRemove.ForEach(v => product.Variations.Remove(v));

        foreach (var d in dtos)
        {
            var entity = d.Id.HasValue
                ? product.Variations.FirstOrDefault(v => v.Id == d.Id.Value)
                : null;
            if (entity == null) { entity = new Variation(); product.Variations.Add(entity); }
            entity.Name = d.Name;
        }
    }

    private void UpdateProductItems(Product product, IList<UpsertProductItemDto> dtos)
    {
        if (dtos == null) { product.ProductItems.Clear(); return; }
        var toRemove = product.ProductItems
            .Where(pi => !dtos.Any(d => d.Id == pi.Id))
            .ToList();
        toRemove.ForEach(pi => product.ProductItems.Remove(pi));

        foreach (var d in dtos)
        {
            var item = d.Id.HasValue
                ? product.ProductItems.FirstOrDefault(pi => pi.Id == d.Id.Value)
                : null;
            if (item == null) { item = new ProductItem(); product.ProductItems.Add(item); }

            item.Sku = d.Sku;
            item.PriceOverride = d.PriceOverride;
            UpdateItemOptions(product, item, d.Options);

            if (item.Inventory == null)
                item.Inventory = new Inventory();
            item.Inventory.QuantityOnHand = d.QuantityOnHand;
            item.Inventory.WarehouseLocation = d.WarehouseLocation;
        }
    }

    private void UpdateItemOptions(Product product, ProductItem item, IList<UpsertProductItemOptionDto> dtos)
    {
        if (dtos == null) { item.Options.Clear(); return; }
        var toRemove = item.Options
            .Where(opt => !dtos.Any(d => d.Id == opt.VariationOption.Id))
            .ToList();
        toRemove.ForEach(opt => item.Options.Remove(opt));

        foreach (var d in dtos)
        {
            var link = item.Options
                .FirstOrDefault(opt => opt.VariationOption.Id == d.Id);
            if (link == null)
            {
                var vo = product.Variations
                    .SelectMany(v => v.Options)
                    .FirstOrDefault(o => o.Id == d.Id)
                    ?? throw new ArgumentException("No variation to attach option");

                link = new ProductItemOption { VariationOption = vo };
                item.Options.Add(link);
            }
            else
            {
                link.VariationOption.Value = d.Value;
                link.VariationOption.PriceModifier = d.PriceModifier;
            }
        }
    }

    private void MapVariations(Product product, IList<UpsertVariationDto> dtos)
    {
        foreach (var dto in dtos)
        {
            var varEntity = dto.Id.HasValue
                ? product.Variations.FirstOrDefault(v => v.Id == dto.Id.Value)
                : null;
            if (varEntity == null) { varEntity = new Variation(); product.Variations.Add(varEntity); }
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
                    optionEntity = product.Variations
                        .SelectMany(v => v.Options)
                        .FirstOrDefault(o => o.Id == opt.Id);

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
