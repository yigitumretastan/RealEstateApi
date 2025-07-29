using RealEstateApi.DTOs;
using RealEstateApi.Models;
using RealEstateApi.Persistence;

namespace RealEstateApi.Services
{
public class PaymentService
{
    private readonly ApplicationDbContext _db;

    public PaymentService(ApplicationDbContext db)
    {
        _db = db;
    }

    public Payment? ProcessPayment(int listingId, PaymentDto dto)
    {
        var listing = _db.Listings.FirstOrDefault(l => l.Id == listingId);
        if (listing == null)
        {
            return null; // İlan yok, ödeme yapılamaz
        }

        var amountToPay = listing.Price;

        var payment = new Payment
        {
            UserId = dto.UserId,
            Amount = amountToPay,
            PaymentMethod = dto.PaymentMethod,
            Description = dto.Description,
            IsSuccessful = amountToPay > 0,
            CreatedAt = DateTime.UtcNow
        };

        return payment;
    }

    public void SavePayment(Payment payment)
    {
        _db.Payments.Add(payment);
        _db.SaveChanges();
    }
}

}
