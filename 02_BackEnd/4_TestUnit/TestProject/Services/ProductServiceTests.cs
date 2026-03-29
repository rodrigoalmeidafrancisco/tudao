using Domain.Entities;
using Domain.Interfaces;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Shared.DTOs;
using Shared.Services;

namespace TestProject.Services;

public class ProductServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IProductRepository> _productRepositoryMock;
    private readonly Mock<ILogger<ProductService>> _loggerMock;
    private readonly ProductService _productService;

    public ProductServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _productRepositoryMock = new Mock<IProductRepository>();
        _loggerMock = new Mock<ILogger<ProductService>>();

        _unitOfWorkMock.Setup(u => u.Products).Returns(_productRepositoryMock.Object);

        _productService = new ProductService(_unitOfWorkMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task WhenGetByIdWithExistingProductThenReturnsProduct()
    {
        var productId = Guid.NewGuid();
        var product = new Product
        {
            Id = productId,
            Name = "Test Product",
            Price = 100
        };

        _productRepositoryMock
            .Setup(r => r.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        var result = await _productService.GetByIdAsync(productId);

        result.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Id.Should().Be(productId);
        result.Data.Name.Should().Be("Test Product");
    }

    [Fact]
    public async Task WhenGetByIdWithNonExistingProductThenReturnsError()
    {
        var productId = Guid.NewGuid();

        _productRepositoryMock
            .Setup(r => r.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product)null);

        var result = await _productService.GetByIdAsync(productId);

        result.Success.Should().BeFalse();
        result.Message.Should().Be("Product not found");
        result.Data.Should().BeNull();
    }

    [Fact]
    public async Task WhenCreateProductThenProductIsCreated()
    {
        var createDto = new CreateProductDto
        {
            Name = "New Product",
            Description = "Description",
            Price = 150,
            Stock = 20,
            Category = "Electronics"
        };

        _productRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product p, CancellationToken ct) => p);

        _unitOfWorkMock
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _productService.CreateAsync(createDto);

        result.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Name.Should().Be("New Product");
        result.Data.Price.Should().Be(150);

        _productRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task WhenUpdateStockWithValidQuantityThenStockIsUpdated()
    {
        var productId = Guid.NewGuid();
        var product = new Product
        {
            Id = productId,
            Name = "Test Product",
            Stock = 10
        };

        _productRepositoryMock
            .Setup(r => r.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        var stockDto = new UpdateStockDto { Quantity = 5 };

        var result = await _productService.UpdateStockAsync(productId, stockDto);

        result.Success.Should().BeTrue();
        result.Data!.Stock.Should().Be(15);

        _productRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task WhenDeleteExistingProductThenProductIsDeleted()
    {
        var productId = Guid.NewGuid();

        _productRepositoryMock
            .Setup(r => r.ExistsAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _productService.DeleteAsync(productId);

        result.Success.Should().BeTrue();
        result.Message.Should().Be("Product deleted successfully");

        _productRepositoryMock.Verify(r => r.DeleteAsync(productId, It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
