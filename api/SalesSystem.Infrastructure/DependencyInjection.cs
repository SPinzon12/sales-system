using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SalesSystem.Application.Interfaces;
using SalesSystem.Application.UseCases;
using SalesSystem.Infrastructure.Data;
using SalesSystem.Infrastructure.Repositories;

namespace SalesSystem.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services, 
        IConfiguration configuration)
    {
        var databasePath = Environment.GetEnvironmentVariable("DATABASE_PATH") 
            ?? "Data Source=sales.db";

        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlite(databasePath));

        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<ISaleRepository, SaleRepository>();
        services.AddScoped<IUserRepository, UserRepository>();

        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddScoped<SaleService>();
        services.AddScoped<AuthService>();

        return services;
    }
}
