using FluentValidation.TestHelper;
using Shared.DTOs;
using Shared.Validators;

namespace TestProject.Validators;

public class ProductValidatorTests
{
    private readonly CreateProductDtoValidator _createValidator;
    private readonly UpdateProductDtoValidator _updateValidator;
    private readonly UpdateStockDtoValidator _stockValidator;

    public ProductValidatorTests()
    {
        _createValidator = new CreateProductDtoValidator();
        _updateValidator = new UpdateProductDtoValidator();
        _stockValidator = new UpdateStockDtoValidator();
    }

    [Fact]
    public void WhenNameIsEmptyThenValidationFails()
    {
        var dto = new CreateProductDto { Name = "", Price = 100, Category = "Test" };

        var result = _createValidator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void WhenNameExceedsMaxLengthThenValidationFails()
    {
        var dto = new CreateProductDto { Name = new string('A', 201), Price = 100, Category = "Test" };

        var result = _createValidator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void WhenPriceIsZeroOrNegativeThenValidationFails()
    {
        var dto = new CreateProductDto { Name = "Test", Price = 0, Category = "Test" };

        var result = _createValidator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.Price);
    }

    [Fact]
    public void WhenStockIsNegativeThenValidationFails()
    {
        var dto = new CreateProductDto { Name = "Test", Price = 100, Stock = -1, Category = "Test" };

        var result = _createValidator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.Stock);
    }

    [Fact]
    public void WhenImageUrlIsInvalidThenValidationFails()
    {
        var dto = new CreateProductDto 
        { 
            Name = "Test", 
            Price = 100, 
            Category = "Test", 
            ImageUrl = "not-a-valid-url" 
        };

        var result = _createValidator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.ImageUrl);
    }

    [Fact]
    public void WhenAllFieldsAreValidThenValidationSucceeds()
    {
        var dto = new CreateProductDto
        {
            Name = "Test Product",
            Description = "Test Description",
            Price = 100,
            Stock = 10,
            Category = "Electronics",
            ImageUrl = "https://example.com/image.jpg"
        };

        var result = _createValidator.TestValidate(dto);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void WhenStockQuantityIsZeroThenValidationFails()
    {
        var dto = new UpdateStockDto { Quantity = 0 };

        var result = _stockValidator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.Quantity);
    }
}
