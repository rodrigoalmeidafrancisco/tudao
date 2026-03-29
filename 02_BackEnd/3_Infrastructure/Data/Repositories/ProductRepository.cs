using Data.Context;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Data.Repositories;

public class ProductRepository : Repository<Product>, IProductRepository
{
    public ProductRepository(DefaultContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Product>> GetByCategoryAsync(string category, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(p => p.Category == category && p.IsActive)
            .OrderBy(p => p.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Product>> GetLowStockProductsAsync(int threshold, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(p => p.Stock <= threshold && p.IsActive)
            .OrderBy(p => p.Stock)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Product>> SearchByNameAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(p => p.Name.Contains(searchTerm) && p.IsActive)
            .OrderBy(p => p.Name)
            .ToListAsync(cancellationToken);
    }
}
