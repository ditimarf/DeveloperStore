namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.CancelItem;

public class CancelItemResponse
{
    public Guid SaleId { get; set; }
    public Guid ItemId { get; set; }
    public bool IsCancelled { get; set; }
    public decimal SaleTotalAmount { get; set; }
}
