using Domain.Entities;
using FluentAssertions;

namespace TestProject.Domain;

public class ProductTests
{
    [Fact]
    public void WhenCreateProductThenIdIsGenerated()
    {
        var product = new Product
        {
            Name = "Test Product",
            Price = 100
        };

        product.Id.Should().NotBeEmpty();
        product.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        product.IsActive.Should().BeTrue();
    }

    [Fact]
    public void WhenUpdateStockWithValidQuantityThenStockIsUpdated()
    {
        var product = new Product
        {
            Name = "Test Product",
            Stock = 10
        };

        product.UpdateStock(5);

        product.Stock.Should().Be(15);
        product.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public void WhenUpdateStockWithNegativeQuantityExceedingStockThenThrowsException()
    {
        var product = new Product
        {
            Name = "Test Product",
            Stock = 5
        };

        var act = () => product.UpdateStock(-10);

        act.Should().Throw<InvalidOperationException>()
           .WithMessage("Insufficient stock");
    }

    [Fact]
    public void WhenUpdatePriceWithValidValueThenPriceIsUpdated()
    {
        var product = new Product
        {
            Name = "Test Product",
            Price = 100
        };

        product.UpdatePrice(150);

        product.Price.Should().Be(150);
        product.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public void WhenUpdatePriceWithNegativeValueThenThrowsException()
    {
        var product = new Product
        {
            Name = "Test Product",
            Price = 100
        };

        var act = () => product.UpdatePrice(-10);

        act.Should().Throw<ArgumentException>()
           .WithMessage("Price cannot be negative*");
    }
}
