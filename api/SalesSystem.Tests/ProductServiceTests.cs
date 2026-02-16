using SalesSystem.Application.DTOs;
using SalesSystem.Application.Interfaces;
using SalesSystem.Application.UseCases;
using SalesSystem.Domain.Entities;
using Moq;

namespace SalesSystem.Tests;

public class ProductServiceTests
{
    private readonly Mock<IProductRepository> _mockProductRepo;
    private readonly ProductService _productService;

    public ProductServiceTests()
    {
        _mockProductRepo = new Mock<IProductRepository>();
        _productService = new ProductService(_mockProductRepo.Object);
    }

    [Fact]
    public async Task CreateProductAsync_ValidData_ReturnsProductDto()
    {
        var dto = new CreateProductDto
        {
            Name = "Test Product",
            Price = 100,
            Stock = 10,
            ImageUrl = "http://test.com/image.jpg"
        };

        _mockProductRepo.Setup(r => r.AddAsync(It.IsAny<Product>()))
            .Returns(Task.CompletedTask);

        var result = await _productService.CreateProductAsync(dto);

        Assert.NotNull(result);
        Assert.Equal(dto.Name, result.Name);
        Assert.Equal(dto.Price, result.Price);
        Assert.Equal(dto.Stock, result.Stock);
        Assert.True(result.IsActive);
        _mockProductRepo.Verify(r => r.AddAsync(It.IsAny<Product>()), Times.Once);
    }

    [Fact]
    public async Task CreateProductAsync_InvalidName_ThrowsArgumentException()
    {
        var dto = new CreateProductDto
        {
            Name = "",
            Price = 100,
            Stock = 10
        };

        await Assert.ThrowsAsync<ArgumentException>(
            () => _productService.CreateProductAsync(dto));
    }

    [Fact]
    public async Task CreateProductAsync_InvalidPrice_ThrowsArgumentException()
    {
        var dto = new CreateProductDto
        {
            Name = "Test",
            Price = 0,
            Stock = 10
        };

        await Assert.ThrowsAsync<ArgumentException>(
            () => _productService.CreateProductAsync(dto));
    }

    [Fact]
    public async Task CreateProductAsync_NegativeStock_ThrowsArgumentException()
    {
        var dto = new CreateProductDto
        {
            Name = "Test",
            Price = 100,
            Stock = -1
        };

        await Assert.ThrowsAsync<ArgumentException>(
            () => _productService.CreateProductAsync(dto));
    }

    [Fact]
    public async Task ListProductsAsync_ReturnsPaginatedResults()
    {
        var products = new List<Product>
        {
            new("Product 1", 100, 10),
            new("Product 2", 200, 20)
        };

        _mockProductRepo.Setup(r => r.GetAllAsync(1, 10))
            .ReturnsAsync(products);
        _mockProductRepo.Setup(r => r.CountAsync())
            .ReturnsAsync(2);

        var result = await _productService.ListProductsAsync(1, 10);

        Assert.NotNull(result);
        Assert.Equal(2, result.Items.Count);
        Assert.Equal(2, result.TotalCount);
        Assert.Equal(1, result.Page);
        Assert.Equal(10, result.PageSize);
    }
}
