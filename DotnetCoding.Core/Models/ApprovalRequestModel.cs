namespace DotnetCoding.Core.Models;

public class ApprovalRequestModel
{
    public int ApprovalQueueId { get; set; }
    public bool Approve { get; set; }
    public string Comments { get; set; } 
}