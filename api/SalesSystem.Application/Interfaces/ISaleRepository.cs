using SalesSystem.Domain.Entities;

namespace SalesSystem.Application.Interfaces;

public interface ISaleRepository
{
    Task<Sale?> GetByIdAsync(Guid id);
    Task<IEnumerable<Sale>> GetAllAsync();
    Task AddAsync(Sale sale);
    Task UpdateAsync(Sale sale);
    Task<IEnumerable<Sale>> GetConfirmedSalesByDateRangeAsync(DateTime from, DateTime to, int page, int pageSize);
    Task<int> CountConfirmedSalesByDateRangeAsync(DateTime from, DateTime to);
    Task<decimal> GetTotalAmountByDateRangeAsync(DateTime from, DateTime to);
}
