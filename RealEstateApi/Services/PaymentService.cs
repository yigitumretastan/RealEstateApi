using RealEstateApi.DTOs;
using RealEstateApi.Models;
using RealEstateApi.Persistence;
using System.Text.RegularExpressions;

namespace RealEstateApi.Services
{
public class PaymentService
{
    private readonly ApplicationDbContext _db;
    private readonly ILogger<PaymentService> _logger;

    public PaymentService(ApplicationDbContext db, ILogger<PaymentService> logger)
    {
        _db = db;
        _logger = logger;
    }

    public Payment? ProcessPayment(int listingId, PaymentDto dto)
    {
        var listing = _db.Listings.FirstOrDefault(l => l.Id == listingId);
        if (listing == null)
        {
            _logger.LogWarning("İlan bulunamadı: {ListingId}", listingId);
            return null;
        }

        // Sadece CreditCard ile işlem yapılacak, değilse hata döndür
        if (dto.PaymentMethod != "CreditCard")
        {
            _logger.LogWarning("Sadece CreditCard ödeme yöntemi desteklenmektedir. Gönderilen: {PaymentMethod}", dto.PaymentMethod);
            return new Payment
            {
                UserId = dto.UserId,
                ListingId = listingId,
                Amount = listing.Price,
                PaymentMethod = dto.PaymentMethod,
                IsSuccessful = false,
                FailureReason = "Sadece kredi kartı ile ödeme yapılabilir."
            };
        }

        // CreditCard doğrulama
        var validationResult = ValidateCreditCard(dto);
        if (!validationResult.IsValid)
        {
            _logger.LogWarning("Kredi kartı doğrulama başarısız: {Reason}", validationResult.ErrorMessage);
            return new Payment
            {
                UserId = dto.UserId,
                ListingId = listingId,
                Amount = listing.Price,
                PaymentMethod = dto.PaymentMethod,
                IsSuccessful = false,
                FailureReason = validationResult.ErrorMessage
            };
        }

        var amountToPay = listing.Price;
        var isPaymentSuccessful = SimulatePaymentProcessing(dto, amountToPay);

        var payment = new Payment
        {
            UserId = dto.UserId,
            ListingId = listingId,
            Amount = amountToPay,
            PaymentMethod = dto.PaymentMethod,
            Description = dto.Description,
            IsSuccessful = isPaymentSuccessful,
            CreatedAt = DateTime.UtcNow,
            TransactionId = GenerateTransactionId()
        };

        if (!string.IsNullOrEmpty(dto.CardNumber))
        {
            payment.MaskedCardNumber = MaskCardNumber(dto.CardNumber);
            payment.CardHolderName = dto.CardHolderName;
            payment.ExpiryDate = dto.ExpiryDate;
            payment.BillingAddress = dto.BillingAddress;
            payment.BillingCity = dto.BillingCity;
            payment.PostalCode = dto.PostalCode;
        }

        if (!isPaymentSuccessful)
        {
            payment.FailureReason = "Ödeme işlemi sırasında teknik bir hata oluştu.";
        }

        _logger.LogInformation("Ödeme işlemi tamamlandı. Başarılı: {IsSuccessful}, TransactionId: {TransactionId}",
            isPaymentSuccessful, payment.TransactionId);

        return payment;
    }

    private ValidationResult ValidateCreditCard(PaymentDto dto)
    {
        if (string.IsNullOrEmpty(dto.CardNumber))
            return new ValidationResult(false, "Kart numarası boş olamaz.");

        if (!IsValidCardNumber(dto.CardNumber.Replace(" ", "").Replace("-", "")))
            return new ValidationResult(false, "Geçersiz kart numarası.");

        if (!IsValidExpiryDate(dto.ExpiryDate))
            return new ValidationResult(false, "Geçersiz son kullanma tarihi.");

        if (string.IsNullOrEmpty(dto.CVV) || !Regex.IsMatch(dto.CVV, @"^[0-9]{3,4}$"))
            return new ValidationResult(false, "Geçersiz CVV kodu.");

        return new ValidationResult(true, "Geçerli");
    }

    private bool IsValidCardNumber(string cardNumber)
    {
        if (string.IsNullOrEmpty(cardNumber) || !cardNumber.All(char.IsDigit))
            return false;

        int sum = 0;
        bool isEven = false;

        for (int i = cardNumber.Length - 1; i >= 0; i--)
        {
            int digit = int.Parse(cardNumber[i].ToString());

            if (isEven)
            {
                digit *= 2;
                if (digit > 9)
                    digit = digit / 10 + digit % 10;
            }

            sum += digit;
            isEven = !isEven;
        }

        return sum % 10 == 0;
    }

    private bool IsValidExpiryDate(string? expiryDate)
    {
        if (string.IsNullOrEmpty(expiryDate))
            return false;

        if (!Regex.IsMatch(expiryDate, @"^(0[1-9]|1[0-2])\/([0-9]{2})$"))
            return false;

        var parts = expiryDate.Split('/');
        var month = int.Parse(parts[0]);
        var year = int.Parse("20" + parts[1]);

        var expiryDateTime = new DateTime(year, month, DateTime.DaysInMonth(year, month));
        return expiryDateTime >= DateTime.Now;
    }

    private bool SimulatePaymentProcessing(PaymentDto dto, decimal amount)
    {
        var random = new Random();

        if (dto.CardNumber?.EndsWith("0000") == true)
            return false;

        var successRate = amount > 1000000 ? 0.7 : 0.9;

        return random.NextDouble() < successRate;
    }

    private string MaskCardNumber(string cardNumber)
    {
        var cleanNumber = cardNumber.Replace(" ", "").Replace("-", "");
        if (cleanNumber.Length < 4)
            return "****";

        return "**** **** **** " + cleanNumber.Substring(cleanNumber.Length - 4);
    }

    private string GenerateTransactionId()
    {
        return $"TXN_{DateTime.UtcNow:yyyyMMddHHmmss}_{Guid.NewGuid().ToString("N")[..8].ToUpper()}";
    }

    public void SavePayment(Payment payment)
    {
        _db.Payments.Add(payment);
        _db.SaveChanges();
    }

    public List<Payment> GetUserPayments(int userId)
    {
        return _db.Payments
            .Where(p => p.UserId == userId)
            .OrderByDescending(p => p.CreatedAt)
            .ToList();
    }

    public Payment? GetPaymentById(int paymentId, int userId)
    {
        return _db.Payments
            .FirstOrDefault(p => p.Id == paymentId && p.UserId == userId);
    }
}

public class ValidationResult
{
    public bool IsValid { get; }
    public string ErrorMessage { get; }

    public ValidationResult(bool isValid, string errorMessage)
    {
        IsValid = isValid;
        ErrorMessage = errorMessage;
    }
}

}