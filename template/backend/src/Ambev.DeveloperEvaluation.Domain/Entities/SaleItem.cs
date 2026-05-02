namespace Ambev.DeveloperEvaluation.Domain.Entities;

public class SaleItem
{
    public Guid Id { get; set; }
    public Guid SaleId { get; set; }
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Discount { get; set; }
    public decimal TotalAmount { get; set; }
    public bool IsCancelled { get; set; }

    public Sale Sale { get; set; } = null!;

    public void ApplyDiscount()
    {
        if (Quantity > 20)
            throw new InvalidOperationException("Cannot sell above 20 identical items.");

        if (Quantity >= 10)
            Discount = 0.20m;
        else if (Quantity >= 4)
            Discount = 0.10m;
        else
            Discount = 0m;

        TotalAmount = (UnitPrice * Quantity) * (1 - Discount);
    }
}
