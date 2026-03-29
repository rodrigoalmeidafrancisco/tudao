using Domain.Entities;
using Domain.Interfaces;
using Shared.DTOs;

namespace Shared.Services;

public class ProductService : IProductService
{
    private readonly IUnitOfWork _unitOfWork;

    public ProductService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<ProductDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var product = await _unitOfWork.Products.GetByIdAsync(id, cancellationToken);

            if (product is null)
            {
                return ApiResponse<ProductDto>.ErrorResponse("Product not found");
            }

            return ApiResponse<ProductDto>.SuccessResponse(MapToDto(product));
        }
        catch (Exception ex)
        {
            return ApiResponse<ProductDto>.ErrorResponse("An error occurred while retrieving the product");
        }
    }

    public async Task<ApiResponse<IEnumerable<ProductDto>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var products = await _unitOfWork.Products.GetAllAsync(cancellationToken);
            var productDtos = products.Select(MapToDto);

            return ApiResponse<IEnumerable<ProductDto>>.SuccessResponse(productDtos);
        }
        catch (Exception ex)
        {
            return ApiResponse<IEnumerable<ProductDto>>.ErrorResponse("An error occurred while retrieving products");
        }
    }

    public async Task<ApiResponse<IEnumerable<ProductDto>>> GetByCategoryAsync(string category, CancellationToken cancellationToken = default)
    {
        try
        {
            var products = await _unitOfWork.Products.GetByCategoryAsync(category, cancellationToken);
            var productDtos = products.Select(MapToDto);

            return ApiResponse<IEnumerable<ProductDto>>.SuccessResponse(productDtos);
        }
        catch (Exception ex)
        {
            return ApiResponse<IEnumerable<ProductDto>>.ErrorResponse("An error occurred while retrieving products");
        }
    }

    public async Task<ApiResponse<IEnumerable<ProductDto>>> SearchByNameAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        try
        {
            var products = await _unitOfWork.Products.SearchByNameAsync(searchTerm, cancellationToken);
            var productDtos = products.Select(MapToDto);

            return ApiResponse<IEnumerable<ProductDto>>.SuccessResponse(productDtos);
        }
        catch (Exception ex)
        {
            return ApiResponse<IEnumerable<ProductDto>>.ErrorResponse("An error occurred while searching products");
        }
    }

    public async Task<ApiResponse<ProductDto>> CreateAsync(CreateProductDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            var product = new Product
            {
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                Stock = dto.Stock,
                Category = dto.Category,
                ImageUrl = dto.ImageUrl
            };

            await _unitOfWork.Products.AddAsync(product, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return ApiResponse<ProductDto>.SuccessResponse(MapToDto(product), "Product created successfully");
        }
        catch (Exception ex)
        {
            return ApiResponse<ProductDto>.ErrorResponse("An error occurred while creating the product");
        }
    }

    public async Task<ApiResponse<ProductDto>> UpdateAsync(Guid id, UpdateProductDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            var product = await _unitOfWork.Products.GetByIdAsync(id, cancellationToken);

            if (product is null)
            {
                return ApiResponse<ProductDto>.ErrorResponse("Product not found");
            }

            product.Name = dto.Name;
            product.Description = dto.Description;
            product.Price = dto.Price;
            product.Stock = dto.Stock;
            product.Category = dto.Category;
            product.ImageUrl = dto.ImageUrl;
            product.IsActive = dto.IsActive;
            product.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.Products.UpdateAsync(product, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return ApiResponse<ProductDto>.SuccessResponse(MapToDto(product), "Product updated successfully");
        }
        catch (Exception ex)
        {
            return ApiResponse<ProductDto>.ErrorResponse("An error occurred while updating the product");
        }
    }

    public async Task<ApiResponse<object>> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var exists = await _unitOfWork.Products.ExistsAsync(id, cancellationToken);

            if (!exists)
            {
                return ApiResponse<object>.ErrorResponse("Product not found");
            }

            await _unitOfWork.Products.DeleteAsync(id, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);


            return ApiResponse<object>.SuccessResponse(null!, "Product deleted successfully");
        }
        catch (Exception ex)
        {
            return ApiResponse<object>.ErrorResponse("An error occurred while deleting the product");
        }
    }

    public async Task<ApiResponse<ProductDto>> UpdateStockAsync(Guid id, UpdateStockDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            var product = await _unitOfWork.Products.GetByIdAsync(id, cancellationToken);

            if (product is null)
            {
                return ApiResponse<ProductDto>.ErrorResponse("Product not found");
            }

            product.UpdateStock(dto.Quantity);

            await _unitOfWork.Products.UpdateAsync(product, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return ApiResponse<ProductDto>.SuccessResponse(MapToDto(product), "Stock updated successfully");
        }
        catch (InvalidOperationException ex)
        {
            return ApiResponse<ProductDto>.ErrorResponse(ex.Message);
        }
        catch (Exception ex)
        {
            return ApiResponse<ProductDto>.ErrorResponse("An error occurred while updating stock");
        }
    }

    private static ProductDto MapToDto(Product product)
    {
        return new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            Stock = product.Stock,
            Category = product.Category,
            ImageUrl = product.ImageUrl,
            IsActive = product.IsActive,
            CreatedAt = product.CreatedAt,
            UpdatedAt = product.UpdatedAt
        };
    }
}
