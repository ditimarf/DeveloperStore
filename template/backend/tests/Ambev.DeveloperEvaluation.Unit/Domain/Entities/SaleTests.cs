using Ambev.DeveloperEvaluation.Domain.Entities;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Entities;

public class SaleTests
{
    [Fact]
    public void Given_QuantityBelow4_When_ApplyDiscount_Then_DiscountShouldBeZero()
    {
        var item = new SaleItem { Quantity = 3, UnitPrice = 10m };
        item.ApplyDiscount();

        Assert.Equal(0m, item.Discount);
        Assert.Equal(30m, item.TotalAmount);
    }

    [Fact]
    public void Given_QuantityBetween4And9_When_ApplyDiscount_Then_DiscountShouldBe10Percent()
    {
        var item = new SaleItem { Quantity = 5, UnitPrice = 10m };
        item.ApplyDiscount();

        Assert.Equal(0.10m, item.Discount);
        Assert.Equal(45m, item.TotalAmount);
    }

    [Fact]
    public void Given_QuantityBetween10And20_When_ApplyDiscount_Then_DiscountShouldBe20Percent()
    {
        var item = new SaleItem { Quantity = 15, UnitPrice = 10m };
        item.ApplyDiscount();

        Assert.Equal(0.20m, item.Discount);
        Assert.Equal(120m, item.TotalAmount);
    }

    [Fact]
    public void Given_QuantityAbove20_When_ApplyDiscount_Then_ShouldThrowException()
    {
        var item = new SaleItem { Quantity = 21, UnitPrice = 10m };

        Assert.Throws<InvalidOperationException>(() => item.ApplyDiscount());
    }

    [Fact]
    public void Given_SaleWithItems_When_CancelItem_Then_TotalShouldBeRecalculated()
    {
        var sale = new Sale();
        var item1 = new SaleItem { Id = Guid.NewGuid(), Quantity = 5, UnitPrice = 10m };
        var item2 = new SaleItem { Id = Guid.NewGuid(), Quantity = 2, UnitPrice = 5m };
        item1.ApplyDiscount();
        item2.ApplyDiscount();
        sale.Items.Add(item1);
        sale.Items.Add(item2);
        sale.RecalculateTotal();

        sale.CancelItem(item1.Id);

        Assert.True(item1.IsCancelled);
        Assert.Equal(10m, sale.TotalAmount);
    }

    [Fact]
    public void Given_Sale_When_Cancel_Then_IsCancelledShouldBeTrue()
    {
        var sale = new Sale();
        sale.Cancel();

        Assert.True(sale.IsCancelled);
        Assert.Single(sale.DomainEvents);
    }

    [Fact]
    public void Given_Sale_When_MarkAsCreated_Then_ShouldHaveSaleCreatedEvent()
    {
        var sale = new Sale();
        sale.MarkAsCreated();

        Assert.Single(sale.DomainEvents);
    }

    [Fact]
    public void Given_Sale_When_MarkAsModified_Then_ShouldHaveSaleModifiedEvent()
    {
        var sale = new Sale();
        sale.MarkAsModified();

        Assert.Single(sale.DomainEvents);
        Assert.NotNull(sale.UpdatedAt);
    }
}
