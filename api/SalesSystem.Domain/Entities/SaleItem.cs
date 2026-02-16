using System;

namespace SalesSystem.Domain.Entities;

public class SaleItem
{
    public Guid Id { get; private set; }
    public Guid SaleId { get; private set; }
    public Guid ProductId { get; private set; }
    public int Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }
    public decimal SubTotal { get; private set; }

    private SaleItem() { }

    public SaleItem(Guid saleId, Guid productId, int quantity, decimal unitPrice)
    {
        if (quantity <= 0)
            throw new ArgumentException("La cantidad debe ser mayor a 0", nameof(quantity));
        
        if (unitPrice <= 0)
            throw new ArgumentException("El precio unitario debe ser mayor a 0", nameof(unitPrice));

        Id = Guid.NewGuid();
        SaleId = saleId;
        ProductId = productId;
        Quantity = quantity;
        UnitPrice = unitPrice;
        SubTotal = quantity * unitPrice;
    }
}
