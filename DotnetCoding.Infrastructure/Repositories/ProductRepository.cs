using DotnetCoding.Core.Interfaces;
using DotnetCoding.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace DotnetCoding.Infrastructure.Repositories
{
    public class ProductRepository : GenericRepository<ProductDetails>, IProductRepository
    {
        public ProductRepository(DbContextClass dbContext) : base(dbContext)
        {

        }

        public async Task<IEnumerable<ProductDetails>> GetAllActiveProducts()
        {
            var query = _dbContext.Products
                .Where(p => p.ProductStatus == "Active")
                .OrderByDescending(p => p.CreationDate); // Sort by posted date in descending order

            return await query.ToListAsync();
        }

        public async Task<IEnumerable<ProductDetails>> GetFilteredProducts(string productName, int? minPrice, int? maxPrice, DateTime? startDate, DateTime? endDate)
        {
            var query = _dbContext.Products.AsQueryable();

            if (!string.IsNullOrEmpty(productName))
            {
                query = query.Where(p => p.ProductName.Contains(productName));
            }

            if (minPrice.HasValue)
            {
                query = query.Where(p => p.ProductPrice >= minPrice.Value);
            }

            if (maxPrice.HasValue)
            {
                query = query.Where(p => p.ProductPrice <= maxPrice.Value);
            }

            if (startDate.HasValue)
            {
                query = query.Where(p => p.CreationDate >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                query = query.Where(p => p.CreationDate <= endDate.Value);
            }

            return await query.ToListAsync();
        }
        public async Task<ProductDetails> GetProductById(int productId)
        {
            return await _dbContext.Products.FindAsync(productId);
        }

        public async Task<int> CreateProduct(ProductDetails product)
        {
            _dbContext.Products.Add(product);
            return await _dbContext.SaveChangesAsync();
        }

        public async Task<int> UpdateProduct(ProductDetails product)
        {
            _dbContext.Products.Update(product);
            return await _dbContext.SaveChangesAsync();
        }

        public async Task<int> DeleteProduct(ProductDetails product)
        {
            _dbContext.Products.Remove(product);
            return await _dbContext.SaveChangesAsync();
        }
        
    }
    
}
