using DotnetCoding.Core.Models;

namespace DotnetCoding.Core.Interfaces;

public interface IApprovalQueueRepository
{
    Task<IEnumerable<ApprovalQueue>> GetAllQueueItems();
    Task<ApprovalQueue> GetQueueItemById(int queueId);
    Task<int> AddToQueue(ApprovalQueue approvalQueue);
    Task<int> UpdateQueueItem(ApprovalQueue approvalQueue);
}