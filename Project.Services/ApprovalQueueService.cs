using DotnetCoding.Core.Interfaces;
using DotnetCoding.Core.Models;
using DotnetCoding.Services.Interfaces;

namespace DotnetCoding.Services;

public class ApprovalQueueService:IApprovalQueueService
{
        private readonly IApprovalQueueRepository _approvalQueueRepository;
        private readonly IProductRepository _productRepository;

        public ApprovalQueueService(IApprovalQueueRepository approvalQueueRepository, IProductRepository productRepository)
        {
            _approvalQueueRepository = approvalQueueRepository;
            _productRepository = productRepository;
        }

        public async Task<IEnumerable<ApprovalQueue>> GetApprovalQueue()
        {
            return await _approvalQueueRepository.GetApprovalQueue();
        }

        public async Task<IEnumerable<ApprovalQueue>> GetAllQueueItems()
        {
            return await _approvalQueueRepository.GetAllQueueItems();
        }

        public async Task PushToQueue(ProductDetails product, string reason, string actionType)
        {
            var approvalQueueItem = new ApprovalQueue
            {
                ProductId = product.Id,
                RequestReason = reason,
                ActionType = actionType,
                Status = "Pending",
                RequestDate = DateTime.Now
            };

            await _approvalQueueRepository.AddToQueue(approvalQueueItem);
            product.ProductStatus = "In Queue"; // Set the product status as 'In Queue'
            await _productRepository.UpdateProduct(product);
        }

        public async Task ProcessApproval(int queueId, bool isApproved, string reviewedBy)
        {
            var queueItem = await _approvalQueueRepository.GetQueueItemById(queueId);
            if (queueItem == null)
            {
                throw new Exception("Queue item not found");
            }

            var product = await _productRepository.GetProductById(queueItem.ProductId);
            if (product == null)
            {
                throw new Exception("Product not found");
            }

            if (isApproved)
            {
                // Approve the request
                if (queueItem.ActionType == "Delete")
                {
                    product.ProductStatus = "Deleted";  // Or actually remove from DB if necessary
                }
                else
                {
                    product.ProductStatus = "Active"; // Product is now approved and active
                }
            }
            else
            {
                // Reject the request, keep the product's status unchanged
                product.ProductStatus = "Active";  // Reset to original state if rejected
            }

            // Update the queue status
            queueItem.Status = isApproved ? "Approved" : "Rejected";
            queueItem.ReviewedBy = reviewedBy;
            queueItem.ReviewedDate = DateTime.Now;

            await _productRepository.UpdateProduct(product);
            await _approvalQueueRepository.UpdateQueueItem(queueItem);
        }

        public async Task ApproveOrReject(int id, bool approve)
        {
            var queueItem = await _approvalQueueRepository.GetQueueItemById(id);
            if (queueItem == null)
                throw new Exception("Approval queue item not found.");

            var product = await _productRepository.GetProductById(queueItem.ProductId);
            if (product == null)
                throw new Exception("Product not found.");

            if (approve)
            {
                // Handle approval logic
                switch (queueItem.ActionType)
                {
                    case "Create":
                        product.ProductStatus = "Active";
                        await _productRepository.CreateProduct(product);
                        break;
                    case "Update":
                        product.ProductStatus = "Active";
                        await _productRepository.UpdateProduct(product);
                        break;
                    case "Delete":
                        await _productRepository.DeleteProduct(product);
                        break;
                }
            }
            else
            {
                // Handle rejection logic
                switch (queueItem.ActionType)
                {
                    case "Create":
                        // Logic to handle rejection of a new product creation
                        break;
                    case "Update":
                        product.ProductStatus = "Active"; // Revert to original state if needed
                        await _productRepository.UpdateProduct(product);
                        break;
                    case "Delete":
                        // Logic to handle rejection of a product deletion
                        break;
                }
            }

            // Remove the item from the approval queue
            await _approvalQueueRepository.RemoveFromQueue(product.Id);
        }
}