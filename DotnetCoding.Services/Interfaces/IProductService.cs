using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotnetCoding.Core.Models;

namespace DotnetCoding.Services.Interfaces
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDetails>> GetAllProducts();
        Task<IEnumerable<ProductDetails>> GetFilteredProducts(string productName, int? minPrice, int? maxPrice, DateTime? startDate, DateTime? endDate);


        Task<IEnumerable<ProductDetails>> GetActiveProducts();
        Task<ProductDetails> GetProduct(int productId);
        Task<int> CreateProduct(ProductDetails product);
        Task<int> UpdateProduct(ProductDetails product);
        Task<int> DeleteProduct(ProductDetails product);
    }
}
