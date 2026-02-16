namespace SalesSystem.Application.DTOs;

public class CreateSaleDto
{
    public Guid UserId { get; set; }
    public List<CreateSaleItemDto> Items { get; set; } = new();
}
