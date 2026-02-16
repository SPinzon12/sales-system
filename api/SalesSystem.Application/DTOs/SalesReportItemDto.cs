namespace SalesSystem.Application.DTOs;

public class SalesReportItemDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public DateTime SaleDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal Total { get; set; }
}
