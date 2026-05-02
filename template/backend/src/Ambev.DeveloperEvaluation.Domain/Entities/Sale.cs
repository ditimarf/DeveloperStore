using Ambev.DeveloperEvaluation.Domain.Events;

namespace Ambev.DeveloperEvaluation.Domain.Entities;

public class Sale
{
    public Guid Id { get; set; }
    public string SaleNumber { get; set; } = string.Empty;
    public DateTime SaleDate { get; set; }
    public Guid CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public Guid BranchId { get; set; }
    public string BranchName { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public bool IsCancelled { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public List<SaleItem> Items { get; set; } = new();
    public List<object> DomainEvents { get; private set; } = new();

    public Sale()
    {
        CreatedAt = DateTime.UtcNow;
        SaleDate = DateTime.UtcNow;
    }

    public void AddItem(SaleItem item)
    {
        item.ApplyDiscount();
        Items.Add(item);
        RecalculateTotal();
    }

    public void RecalculateTotal()
    {
        TotalAmount = Items
            .Where(i => !i.IsCancelled)
            .Sum(i => i.TotalAmount);
    }

    public void Cancel()
    {
        IsCancelled = true;
        UpdatedAt = DateTime.UtcNow;
        DomainEvents.Add(new SaleCancelledEvent(this));
    }

    public void CancelItem(Guid itemId)
    {
        var item = Items.FirstOrDefault(i => i.Id == itemId);
        if (item == null)
            throw new KeyNotFoundException($"Item with ID {itemId} not found in sale.");

        item.IsCancelled = true;
        RecalculateTotal();
        UpdatedAt = DateTime.UtcNow;
        DomainEvents.Add(new ItemCancelledEvent(this, item));
    }

    public void MarkAsCreated()
    {
        DomainEvents.Add(new SaleCreatedEvent(this));
    }

    public void MarkAsModified()
    {
        UpdatedAt = DateTime.UtcNow;
        DomainEvents.Add(new SaleModifiedEvent(this));
    }
}
