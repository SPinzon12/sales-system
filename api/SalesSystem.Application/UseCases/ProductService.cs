using SalesSystem.Application.DTOs;
using SalesSystem.Application.Interfaces;
using SalesSystem.Domain.Entities;

namespace SalesSystem.Application.UseCases;

public class ProductService
{
    private readonly IProductRepository _productRepository;

    public ProductService(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<ProductDto> CreateProductAsync(CreateProductDto dto)
    {
        var product = new Product(dto.Name, dto.Price, dto.Stock, dto.ImageUrl);
        
        await _productRepository.AddAsync(product);
        
        return new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Price = product.Price,
            Stock = product.Stock,
            ImageUrl = product.ImageUrl,
            IsActive = product.IsActive
        };
    }

    public async Task<PaginationResult<ProductDto>> ListProductsAsync(int page, int pageSize)
    {
        var products = await _productRepository.GetAllAsync(page, pageSize);
        var totalCount = await _productRepository.CountAsync();
        
        var items = products.Select(p => new ProductDto
        {
            Id = p.Id,
            Name = p.Name,
            Price = p.Price,
            Stock = p.Stock,
            ImageUrl = p.ImageUrl,
            IsActive = p.IsActive
        }).ToList();
        
        return new PaginationResult<ProductDto>
        {
            Items = items,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }
}
