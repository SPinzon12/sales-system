using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SalesSystem.Application.DTOs;
using SalesSystem.Application.UseCases;

namespace SalesSystem.Api.Controllers;

[ApiController]
[Route("api/sales")]
[Authorize]
public class SalesController : ControllerBase
{
    private readonly SaleService _saleService;
    private readonly ILogger<SalesController> _logger;

    public SalesController(SaleService saleService, ILogger<SalesController> logger)
    {
        _saleService = saleService;
        _logger = logger;
    }

    [HttpPost]
    public async Task<ActionResult<SaleDto>> Create([FromBody] CreateSaleDto dto)
    {
        var sale = await _saleService.RegisterSaleAsync(dto);
        
        _logger.LogInformation("Venta registrada exitosamente: {SaleId}, Total: {Total}", 
            sale.Id, sale.Total);
        
        return CreatedAtAction(nameof(GetById), new { id = sale.Id }, sale);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<SaleDto>> GetById(Guid id)
    {
        try
        {
            var sale = await _saleService.GetByIdAsync(id);
            return Ok(sale);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { error = ex.Message });
        }
    }

    [HttpGet("report")]
    public async Task<ActionResult<SalesReportDto>> GetReport(
        [FromQuery] DateTime from,
        [FromQuery] DateTime to,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        if (from > to)
        {
            return BadRequest(new { error = "La fecha 'from' no puede ser mayor a 'to'" });
        }

        if (page < 1)
        {
            return BadRequest(new { error = "La pagina debe ser mayor a 0" });
        }

        if (pageSize < 1)
        {
            return BadRequest(new { error = "El tamaño de pagina debe ser mayor a 0" });
        }

        const int maxPageSize = 50;
        if (pageSize > maxPageSize)
        {
            pageSize = maxPageSize;
        }

        var report = await _saleService.GetSalesReportAsync(from, to, page, pageSize);
        
        _logger.LogInformation("Reporte de ventas generado: {From} - {To}, Pagina: {Page}", 
            from.ToString("yyyy-MM-dd"), to.ToString("yyyy-MM-dd"), page);
        
        return Ok(report);
    }
}
