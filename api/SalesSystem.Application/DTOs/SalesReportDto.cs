namespace SalesSystem.Application.DTOs;

public class SalesReportDto
{
    public List<SalesReportItemDto> Items { get; set; } = new();
    public PaginationInfo Pagination { get; set; } = new();
    public SalesReportSummary Summary { get; set; } = new();
}

public class PaginationInfo
{
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }
}

public class SalesReportSummary
{
    public decimal TotalAmount { get; set; }
}
