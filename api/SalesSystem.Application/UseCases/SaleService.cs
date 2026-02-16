using Microsoft.Extensions.Logging;
using SalesSystem.Application.DTOs;
using SalesSystem.Application.Interfaces;
using SalesSystem.Domain.Entities;

namespace SalesSystem.Application.UseCases;

public class SaleService
{
    private readonly ISaleRepository _saleRepository;
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<SaleService> _logger;

    public SaleService(
        ISaleRepository saleRepository, 
        IProductRepository productRepository, 
        IUnitOfWork unitOfWork,
        ILogger<SaleService> logger)
    {
        _saleRepository = saleRepository;
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<SaleDto> RegisterSaleAsync(CreateSaleDto dto)
    {
        _logger.LogInformation("Iniciando registro de venta para usuario: {UserId}, Items: {ItemCount}", 
            dto.UserId, dto.Items.Count);

        var sale = new Sale(dto.UserId);
        
        foreach (var item in dto.Items)
        {
            var product = await _productRepository.GetByIdAsync(item.ProductId);
            if (product == null)
            {
                _logger.LogError("Producto no encontrado: {ProductId}", item.ProductId);
                throw new InvalidOperationException($"Producto {item.ProductId} no encontrado");
            }
            
            if (!product.IsActive)
            {
                _logger.LogWarning("Producto inactivo: {ProductId} - {ProductName}", product.Id, product.Name);
                throw new InvalidOperationException($"El producto {product.Name} no esta activo");
            }
            
            if (product.Stock < item.Quantity)
            {
                _logger.LogWarning("Stock insuficiente para producto: {ProductId}, Stock actual: {Stock}, Solicitado: {Quantity}",
                    product.Id, product.Stock, item.Quantity);
                throw new InvalidOperationException($"Stock insuficiente para el producto {product.Name}");
            }
            
            sale.AddItem(product, item.Quantity);
            product.DecreaseStock(item.Quantity);
            await _productRepository.UpdateAsync(product);
            
            _logger.LogDebug("Stock decremented for product: {ProductId}, NewStock: {Stock}", 
                product.Id, product.Stock);
        }
        
        sale.Confirm();
        
        await _saleRepository.AddAsync(sale);
        await _unitOfWork.SaveChangesAsync();
        
        _logger.LogInformation("Venta registrada exitosamente: {SaleId}, Total: {Total}", sale.Id, sale.Total);
        
        return MapToDto(sale);
    }

    public async Task<SaleDto> ConfirmSaleAsync(Guid saleId)
    {
        var sale = await _saleRepository.GetByIdAsync(saleId);
        if (sale == null)
        {
            _logger.LogError("Venta no encontrada: {SaleId}", saleId);
            throw new InvalidOperationException("Venta no encontrada");
        }
        
        sale.Confirm();
        await _saleRepository.UpdateAsync(sale);
        
        _logger.LogInformation("Venta confirmada: {SaleId}", saleId);
        
        return MapToDto(sale);
    }

    public async Task<SaleDto> GetByIdAsync(Guid id)
    {
        var sale = await _saleRepository.GetByIdAsync(id);
        if (sale == null)
        {
            _logger.LogError("Venta no encontrada: {SaleId}", id);
            throw new InvalidOperationException("Venta no encontrada");
        }
        
        _logger.LogInformation("Venta encontrada: {SaleId}", id);
        
        return new SaleDto
        {
            Id = sale.Id,
            UserId = sale.UserId,
            SaleDate = sale.SaleDate,
            Status = sale.Status.ToString(),
            Total = sale.Total,
            Items = sale.SaleItems.Select(i => new SaleItemDto
            {
                ProductId = i.ProductId,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice,
                SubTotal = i.SubTotal
            }).ToList()
        };
    }

    public async Task<SalesReportDto> GetSalesReportAsync(DateTime from, DateTime to, int page, int pageSize)
    {
        _logger.LogInformation("Generando reporte de ventas: {From} - {To}, Pagina: {Page}, Tamano: {PageSize}", 
            from.ToString("yyyy-MM-dd"), to.ToString("yyyy-MM-dd"), page, pageSize);

        var fromDate = from.Date;
        var toDate = to.Date.AddDays(1).AddTicks(-1);

        var sales = await _saleRepository.GetConfirmedSalesByDateRangeAsync(fromDate, toDate, page, pageSize);
        var totalCount = await _saleRepository.CountConfirmedSalesByDateRangeAsync(fromDate, toDate);
        var totalAmount = await _saleRepository.GetTotalAmountByDateRangeAsync(fromDate, toDate);

        var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

        _logger.LogInformation("Reporte generado: {Count} ventas, Monto total: {TotalAmount}", totalCount, totalAmount);

        return new SalesReportDto
        {
            Items = sales.Select(s => new SalesReportItemDto
            {
                Id = s.Id,
                UserId = s.UserId,
                SaleDate = s.SaleDate,
                Status = s.Status.ToString(),
                Total = s.Total
            }).ToList(),
            Pagination = new PaginationInfo
            {
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = totalPages
            },
            Summary = new SalesReportSummary
            {
                TotalAmount = totalAmount
            }
        };
    }

    private SaleDto MapToDto(Sale sale)
    {
        return new SaleDto
        {
            Id = sale.Id,
            UserId = sale.UserId,
            SaleDate = sale.SaleDate,
            Status = sale.Status.ToString(),
            Total = sale.Total,
            Items = sale.SaleItems.Select(i => new SaleItemDto
            {
                ProductId = i.ProductId,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice,
                SubTotal = i.SubTotal
            }).ToList()
        };
    }
}
