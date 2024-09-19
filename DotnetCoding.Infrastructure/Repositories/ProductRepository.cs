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
            return await _dbContext.Products
                .Where(p => p.ProductStatus == "Active")
                .OrderByDescending(p => p.Id)
                .ToListAsync();
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
