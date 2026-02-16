using System;

namespace SalesSystem.Domain.Entities;

public class Product
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public decimal Price { get; private set; }
    public int Stock { get; private set; }
    public string? ImageUrl { get; private set; }
    public bool IsActive { get; private set; }

    private Product() { }

    public Product(string name, decimal price, int stock, string? imageUrl = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("El nombre no puede estar vacio", nameof(name));
        
        if (price <= 0)
            throw new ArgumentException("El precio debe ser mayor a 0", nameof(price));
        
        if (stock < 0)
            throw new ArgumentException("El stock no puede ser negativo", nameof(stock));

        Id = Guid.NewGuid();
        Name = name;
        Price = price;
        Stock = stock;
        ImageUrl = imageUrl;
        IsActive = true;
    }

    public void IncreaseStock(int amount)
    {
        if (amount <= 0)
            throw new ArgumentException("La cantidad debe ser mayor a 0", nameof(amount));
        
        Stock += amount;
    }

    public void DecreaseStock(int amount)
    {
        if (amount <= 0)
            throw new ArgumentException("La cantidad debe ser mayor a 0", nameof(amount));
        
        if (amount > Stock)
            throw new InvalidOperationException("Stock insuficiente");
        
        Stock -= amount;
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    public void Activate()
    {
        IsActive = true;
    }

    public void UpdatePrice(decimal newPrice)
    {
        if (newPrice <= 0)
            throw new ArgumentException("El precio debe ser mayor a 0", nameof(newPrice));
        
        Price = newPrice;
    }

    public void Update(string name, decimal price, int stock, string? imageUrl, bool isActive)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("El nombre no puede estar vacio", nameof(name));
        
        if (price <= 0)
            throw new ArgumentException("El precio debe ser mayor a 0", nameof(price));
        
        if (stock < 0)
            throw new ArgumentException("El stock no puede ser negativo", nameof(stock));

        Name = name;
        Price = price;
        Stock = stock;
        ImageUrl = imageUrl;
        IsActive = isActive;
    }
}
