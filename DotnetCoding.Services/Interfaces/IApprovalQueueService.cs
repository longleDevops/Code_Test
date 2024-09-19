using DotnetCoding.Core.Models;

namespace DotnetCoding.Services.Interfaces;

public interface IApprovalQueueService
{
   
        Task PushToQueue(ProductDetails product, string reason, string actionType);
        Task<IEnumerable<ApprovalQueue>> GetApprovalQueue();

        Task<IEnumerable<ApprovalQueue>> GetAllQueueItems();
        Task ProcessApproval(int queueId, bool isApproved, string reviewedBy);
        Task ApproveOrReject(int approvalQueueId, bool approve);

}