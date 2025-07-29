using System;

namespace RealEstateApi.Models
{
    public class Payment
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public decimal Amount { get; set; }

        public string PaymentMethod { get; set; } = string.Empty;

        public string? Description { get; set; }

        public string TransactionId { get; set; } = Guid.NewGuid().ToString();

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public bool IsSuccessful { get; set; }
    }
}
