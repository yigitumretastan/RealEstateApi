using System;
using System.ComponentModel.DataAnnotations;

namespace RealEstateApi.Models
{
    public class Payment
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public int ListingId { get; set; }

        public decimal Amount { get; set; }

        [Required]
        public string PaymentMethod { get; set; } = string.Empty;

    
        public string? MaskedCardNumber { get; set; }

        public string? CardHolderName { get; set; }

        public string? ExpiryDate { get; set; }

        public string? BillingAddress { get; set; }

        public string? BillingCity { get; set; }

        public string? PostalCode { get; set; }

        public string? Description { get; set; }

        public string TransactionId { get; set; } = Guid.NewGuid().ToString();

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public bool IsSuccessful { get; set; }

        public string? FailureReason { get; set; } 
        public User? User { get; set; }
        public Listing? Listing { get; set; }
    }
}