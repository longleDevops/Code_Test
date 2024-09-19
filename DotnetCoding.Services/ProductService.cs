using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotnetCoding.Core.Interfaces;
using DotnetCoding.Core.Models;
using DotnetCoding.Infrastructure.Repositories;
using DotnetCoding.Services.Interfaces;

namespace DotnetCoding.Services
{
    public class ProductService : IProductService
    {
        public IUnitOfWork _unitOfWork;
        private readonly IApprovalQueueRepository _approvalQueueRepository;

        public ProductService(IUnitOfWork unitOfWork, IApprovalQueueRepository approvalQueueRepository)
        {
            _unitOfWork = unitOfWork;
            _approvalQueueRepository = approvalQueueRepository;
        }

        public async Task<IEnumerable<ProductDetails>> GetAllProducts()
        {
            var productDetailsList = await _unitOfWork.Products.GetAll();
            return productDetailsList;
        }

        public async Task<IEnumerable<ProductDetails>> GetActiveProducts()
        {
            var activeProducts = await _unitOfWork.Products.GetAllActiveProducts();
            return activeProducts;
        }

        public async Task<ProductDetails> GetProduct(int productId)
        {
            return await _unitOfWork.Products.GetProductById(productId);
        }

        public async Task<int> CreateProduct(ProductDetails product)
        {
            if (product.ProductPrice > 10000)
                throw new Exception("Product price cannot exceed $10,000.");

            if (product.ProductPrice > 5000)
            {
                await _approvalQueueRepository.AddToQueue(new ApprovalQueue
                {
                    Product = product,
                    RequestReason = "Price exceeds $5000",
                    ActionType = "Create",
                    RequestDate = DateTime.Now,
                    Status = "Pending"
                });
                product.ProductStatus = "In Queue";
            }
            else
            {
                product.ProductStatus = "Active";
            }

            return await _unitOfWork.Products.CreateProduct(product);
        }

        public async Task<int> UpdateProduct(ProductDetails product)
        {
            var existingProduct = await _unitOfWork.Products.GetProductById(product.Id);
            if (existingProduct == null)
                throw new Exception("Product not found.");

            if (product.ProductPrice > existingProduct.ProductPrice * 1.5)
            {
                await _approvalQueueRepository.AddToQueue(new ApprovalQueue
                {
                    Product = product,
                    RequestReason = "Price increased by more than 50%",
                    ActionType = "Update",
                    RequestDate = DateTime.Now,
                    Status = "Pending"
                });
                product.ProductStatus = "In Queue";
            }

            return await _unitOfWork.Products.UpdateProduct(product);
        }

        public async Task<int> DeleteProduct(ProductDetails productDetails)
        {
            var product = await _unitOfWork.Products.GetProductById(productDetails.Id);
            if (product == null)
                throw new Exception("Product not found.");

            // Add to the approval queue with a delete request
            await _approvalQueueRepository.AddToQueue(new ApprovalQueue
            {
                ProductId = product.Id,
                Product = product,
                RequestReason = "Request to delete product",
                ActionType = "Delete",
                RequestDate = DateTime.Now,
                Status = "Pending"
            });

            // Update product status to indicate it is in the queue for deletion
            product.ProductStatus = "In Queue";
            return await _unitOfWork.Products.UpdateProduct(product);
        }
    }
}
