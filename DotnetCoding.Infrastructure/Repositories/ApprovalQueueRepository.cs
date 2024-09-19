using DotnetCoding.Core.Interfaces;
using DotnetCoding.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace DotnetCoding.Infrastructure.Repositories;

public class ApprovalQueueRepository : IApprovalQueueRepository
{
    private readonly DbContextClass _dbContext;

    public ApprovalQueueRepository(DbContextClass dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<ApprovalQueue>> GetAllQueueItems()
    {
        return await _dbContext.ApprovalQueues
            .Include(a => a.Product) // Include related Product
            .OrderBy(a => a.RequestDate)
            .ToListAsync();
    }

    public async Task<ApprovalQueue> GetQueueItemById(int queueId)
    {
        return await _dbContext.ApprovalQueues
            .Include(a => a.Product)
            .FirstOrDefaultAsync(a => a.QueueId == queueId);
    }

    public async Task<int> AddToQueue(ApprovalQueue approvalQueue)
    {
        _dbContext.ApprovalQueues.Add(approvalQueue);
        return await _dbContext.SaveChangesAsync();
    }

    public async Task<int> UpdateQueueItem(ApprovalQueue approvalQueue)
    {
        _dbContext.ApprovalQueues.Update(approvalQueue);
        return await _dbContext.SaveChangesAsync();
    }
}