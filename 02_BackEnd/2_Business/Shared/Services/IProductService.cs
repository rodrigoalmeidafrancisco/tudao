using Shared.DTOs;

namespace Shared.Services;

/// <summary>
/// Product service interface
/// </summary>
public interface IProductService
{
    Task<ApiResponse<ProductDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ApiResponse<IEnumerable<ProductDto>>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<ApiResponse<IEnumerable<ProductDto>>> GetByCategoryAsync(string category, CancellationToken cancellationToken = default);
    Task<ApiResponse<IEnumerable<ProductDto>>> SearchByNameAsync(string searchTerm, CancellationToken cancellationToken = default);
    Task<ApiResponse<ProductDto>> CreateAsync(CreateProductDto dto, CancellationToken cancellationToken = default);
    Task<ApiResponse<ProductDto>> UpdateAsync(Guid id, UpdateProductDto dto, CancellationToken cancellationToken = default);
    Task<ApiResponse<object>> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ApiResponse<ProductDto>> UpdateStockAsync(Guid id, UpdateStockDto dto, CancellationToken cancellationToken = default);
}
