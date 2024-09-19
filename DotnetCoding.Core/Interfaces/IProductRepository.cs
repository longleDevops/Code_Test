using DotnetCoding.Core.Models;

namespace DotnetCoding.Core.Interfaces
{
    public interface IProductRepository : IGenericRepository<ProductDetails>
    {
        Task<ProductDetails> GetProductById(int productId);
        Task<IEnumerable<ProductDetails>> GetFilteredProducts(string productName, int? minPrice, int? maxPrice, DateTime? startDate, DateTime? endDate);

        Task<IEnumerable<ProductDetails>> GetAllActiveProducts();

        Task <int> CreateProduct(ProductDetails product);
        Task <int> UpdateProduct(ProductDetails product);
        Task <int> DeleteProduct(ProductDetails product);
    }
}
