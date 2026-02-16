using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SalesSystem.Application.DTOs;
using SalesSystem.Application.Interfaces;
using SalesSystem.Domain.Entities;

namespace SalesSystem.Api.Controllers;

[ApiController]
[Route("api/products")]
public class ProductsController : ControllerBase
{
    private readonly IProductRepository _productRepository;
    private readonly ILogger<ProductsController> _logger;

    public ProductsController(IProductRepository productRepository, ILogger<ProductsController> logger)
    {
        _productRepository = productRepository;
        _logger = logger;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetAll()
    {
        var products = await _productRepository.GetAllAsync(1, 100);
        
        var productDtos = products.Select(p => new ProductDto
        {
            Id = p.Id,
            Name = p.Name,
            Price = p.Price,
            Stock = p.Stock,
            ImageUrl = p.ImageUrl,
            IsActive = p.IsActive
        });

        _logger.LogInformation("Listado de productos retrieved: {Count} productos", products.Count());
        return Ok(productDtos);
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<ProductDto>> GetById(Guid id)
    {
        var product = await _productRepository.GetByIdAsync(id);
        
        if (product == null)
            return NotFound(new { error = "Producto no encontrado" });

        var productDto = new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Price = product.Price,
            Stock = product.Stock,
            ImageUrl = product.ImageUrl,
            IsActive = product.IsActive
        };

        return Ok(productDto);
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<ProductDto>> Create([FromBody] CreateProductDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
            return BadRequest(new { error = "El nombre es requerido" });

        if (dto.Price <= 0)
            return BadRequest(new { error = "El precio debe ser mayor a 0" });

        if (dto.Stock < 0)
            return BadRequest(new { error = "El stock no puede ser negativo" });

        var product = new Product(dto.Name, dto.Price, dto.Stock, dto.ImageUrl);
        
        await _productRepository.AddAsync(product);

        _logger.LogInformation("Producto creado exitosamente: {ProductId} - {ProductName}", product.Id, product.Name);

        var response = new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Price = product.Price,
            Stock = product.Stock,
            ImageUrl = product.ImageUrl,
            IsActive = product.IsActive
        };

        return CreatedAtAction(nameof(GetById), new { id = product.Id }, response);
    }

    [HttpPut("{id}")]
    [Authorize]
    public async Task<ActionResult<ProductDto>> Update(Guid id, [FromBody] UpdateProductDto dto)
    {
        var product = await _productRepository.GetByIdAsync(id);
        
        if (product == null)
            return NotFound(new { error = "Producto no encontrado" });

        if (string.IsNullOrWhiteSpace(dto.Name))
            return BadRequest(new { error = "El nombre es requerido" });

        if (dto.Price <= 0)
            return BadRequest(new { error = "El precio debe ser mayor a 0" });

        if (dto.Stock < 0)
            return BadRequest(new { error = "El stock no puede ser negativo" });

        product.Update(dto.Name, dto.Price, dto.Stock, dto.ImageUrl, dto.IsActive);
        
        await _productRepository.UpdateAsync(product);

        _logger.LogInformation("Producto actualizado: {ProductId} - {ProductName}", product.Id, product.Name);

        var response = new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Price = product.Price,
            Stock = product.Stock,
            ImageUrl = product.ImageUrl,
            IsActive = product.IsActive
        };

        return Ok(response);
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> Delete(Guid id)
    {
        var product = await _productRepository.GetByIdAsync(id);
        
        if (product == null)
            return NotFound(new { error = "Producto no encontrado" });

        product.Deactivate();
        await _productRepository.UpdateAsync(product);

        _logger.LogInformation("Producto desactivado (soft delete): {ProductId}", id);

        return NoContent();
    }

    [HttpPatch("{id}/reactivate")]
    [Authorize]
    public async Task<ActionResult<ProductDto>> Reactivate(Guid id)
    {
        var product = await _productRepository.GetByIdAsync(id);
        
        if (product == null)
            return NotFound(new { error = "Producto no encontrado" });

        if (product.IsActive)
            return BadRequest(new { error = "El producto ya esta activo" });

        product.Activate();
        await _productRepository.UpdateAsync(product);

        _logger.LogInformation("Producto reactivado: {ProductId}", id);

        var response = new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Price = product.Price,
            Stock = product.Stock,
            ImageUrl = product.ImageUrl,
            IsActive = product.IsActive
        };

        return Ok(response);
    }
}
