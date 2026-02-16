using Microsoft.EntityFrameworkCore;
using SalesSystem.Application.Interfaces;
using SalesSystem.Domain.Entities;
using SalesSystem.Infrastructure.Data;

namespace SalesSystem.Infrastructure.Repositories;

public class SaleRepository : ISaleRepository
{
    private readonly AppDbContext _context;

    public SaleRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Sale?> GetByIdAsync(Guid id)
    {
        return await _context.Sales
            .Include(s => s.SaleItems)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<IEnumerable<Sale>> GetAllAsync()
    {
        return await _context.Sales
            .Include(s => s.SaleItems)
            .ToListAsync();
    }

    public async Task AddAsync(Sale sale)
    {
        await _context.Sales.AddAsync(sale);
    }

    public async Task UpdateAsync(Sale sale)
    {
        _context.Sales.Update(sale);
    }

    public async Task<IEnumerable<Sale>> GetConfirmedSalesByDateRangeAsync(DateTime from, DateTime to, int page, int pageSize)
    {
        return await _context.Sales
            .Where(s => s.Status == SaleStatus.Confirmed && s.SaleDate >= from && s.SaleDate <= to)
            .OrderByDescending(s => s.SaleDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<int> CountConfirmedSalesByDateRangeAsync(DateTime from, DateTime to)
    {
        return await _context.Sales
            .Where(s => s.Status == SaleStatus.Confirmed && s.SaleDate >= from && s.SaleDate <= to)
            .CountAsync();
    }

    public async Task<decimal> GetTotalAmountByDateRangeAsync(DateTime from, DateTime to)
    {
        var sales = await _context.Sales
            .Where(s => s.Status == SaleStatus.Confirmed && s.SaleDate >= from && s.SaleDate <= to)
            .ToListAsync();
        return sales.Sum(s => s.Total);
    }
}
