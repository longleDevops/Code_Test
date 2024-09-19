using DotnetCoding.Core.Models;

namespace DotnetCoding.Services.Interfaces;

public interface IApprovalQueueService
{
   
        Task PushToQueue(ProductDetails product, string reason, string actionType);
        Task<IEnumerable<ApprovalQueue>> GetAllQueueItems();
        Task ProcessApproval(int queueId, bool isApproved, string reviewedBy);
}