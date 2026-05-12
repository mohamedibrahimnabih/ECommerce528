using System.ComponentModel.DataAnnotations;

namespace ECommerce528.Models
{
    public enum PaymentMethod 
    {
        Visa = 1,
        COD,
    }

    public enum PaymentStatus
    {
        Pending = 1,
        Completed,
        Refunded
    }

    public enum OrderStatus
    {
        Pending = 1,
        InProcessing,
        Shipped,
        OnWay,
        Completed,
        Canceled
    }

    public class Order
    {
        public int Id { get; set; }

        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }

        [Editable(false)]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public OrderStatus OrderStatus { get; set; } = OrderStatus.Pending;
        public decimal TotalPrice { get; set; }

        public string SessionId { get; set; } = string.Empty;
        public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.Visa;
        public string? TransactionId { get; set; }
        public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Pending;

        public DateTime? ShippedAt { get; set; }
        public string? CarrierName { get; set; }
        public string? TrackingNumber { get; set; }
    }
}
