using System;
using System.Collections.Generic;
using System.Linq;

namespace SalesSystem.Domain.Entities;

public enum SaleStatus
{
    Draft,
    Confirmed,
    Cancelled
}

public class Sale
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public DateTime SaleDate { get; private set; }
    public SaleStatus Status { get; private set; }
    public decimal Total { get; private set; }
    
    private readonly List<SaleItem> _saleItems = new();
    public IReadOnlyCollection<SaleItem> SaleItems => _saleItems.AsReadOnly();

    private Sale() { }

    public Sale(Guid userId)
    {
        Id = Guid.NewGuid();
        UserId = userId;
        SaleDate = DateTime.UtcNow;
        Status = SaleStatus.Draft;
        Total = 0;
    }

    public void AddItem(Product product, int quantity)
    {
        if (Status != SaleStatus.Draft)
            throw new InvalidOperationException("Solo se pueden agregar items a ventas en borrador");
        
        if (quantity <= 0)
            throw new ArgumentException("La cantidad debe ser mayor a 0", nameof(quantity));
        
        if (!product.IsActive)
            throw new InvalidOperationException($"El producto '{product.Name}' no esta activo");
        
        if (product.Stock < quantity)
            throw new InvalidOperationException(
                $"Stock insuficiente para el producto '{product.Name}'. " +
                $"Disponible: {product.Stock}, Solicitado: {quantity}");

        var existingItem = _saleItems.FirstOrDefault(i => i.ProductId == product.Id);
        
        if (existingItem != null)
        {
            var newQuantity = existingItem.Quantity + quantity;
            _saleItems.Remove(existingItem);
            _saleItems.Add(new SaleItem(Id, product.Id, newQuantity, product.Price));
        }
        else
        {
            _saleItems.Add(new SaleItem(Id, product.Id, quantity, product.Price));
        }

        CalculateTotal();
    }

    public void RemoveItem(Guid productId)
    {
        if (Status != SaleStatus.Draft)
            throw new InvalidOperationException("Solo se pueden remover items de ventas en borrador");
        
        var item = _saleItems.FirstOrDefault(i => i.ProductId == productId);
        if (item != null)
        {
            _saleItems.Remove(item);
            CalculateTotal();
        }
    }

    public void Confirm()
    {
        if (Status != SaleStatus.Draft)
            throw new InvalidOperationException("Solo se pueden confirmar ventas en borrador");
        
        if (!_saleItems.Any())
            throw new InvalidOperationException("No se puede confirmar una venta sin items");
        
        Status = SaleStatus.Confirmed;
    }

    public void Cancel()
    {
        if (Status == SaleStatus.Cancelled)
            throw new InvalidOperationException("La venta ya esta cancelada");
        
        Status = SaleStatus.Cancelled;
    }

    private void CalculateTotal()
    {
        Total = _saleItems.Sum(i => i.SubTotal);
    }
}
