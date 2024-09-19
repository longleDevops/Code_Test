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

    public async Task<IEnumerable<ApprovalQueue>> GetApprovalQueue()
    {
        return await _dbContext.ApprovalQueues
            .OrderBy(a => a.RequestDate) // Sort by request date in ascending order
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
    
    public async Task ApproveOrReject(int id, bool approve)
    {
        var approvalItem = await _dbContext.ApprovalQueues.FindAsync(id);
        if (approvalItem == null) return;

        if (approve)
        {
            approvalItem.Status = "Approved";
            var product = await _dbContext.Products.FindAsync(approvalItem.ProductId);

            if (product == null) return;

            switch (approvalItem.ActionType)
            {
                case "Create":
                    product.ProductStatus = "Active";
                    _dbContext.Products.Add(product);
                    break;
                case "Update":
                    product.ProductStatus = "Active";
                    _dbContext.Products.Update(product);
                    break;
                case "Delete":
                    _dbContext.Products.Remove(product);
                    break;
            }
        }
        else
        {
            approvalItem.Status = "Rejected";
        }

        _dbContext.ApprovalQueues.Update(approvalItem);
        await _dbContext.SaveChangesAsync();
    }

}