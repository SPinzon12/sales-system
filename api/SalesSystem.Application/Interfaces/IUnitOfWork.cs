namespace SalesSystem.Application.Interfaces;

public interface IUnitOfWork
{
    Task<int> SaveChangesAsync();
}
