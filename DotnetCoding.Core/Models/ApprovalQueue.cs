namespace DotnetCoding.Core.Models;

public class ApprovalQueue
{
 
        public int QueueId { get; set; }

        public int ProductId { get; set; }
        public ProductDetails Product { get; set; } // Navigation property

        public string RequestReason { get; set; }

        public DateTime RequestDate { get; set; }

        public string ActionType { get; set; } // E.g., "Create", "Update", "Delete"

        public string Status { get; set; } // E.g., "Pending", "Approved", "Rejected"

        public string ReviewedBy { get; set; } // Optional: The admin who reviewed the request

        public DateTime? ReviewedDate { get; set; } // Optional: Date when the request was processed

}