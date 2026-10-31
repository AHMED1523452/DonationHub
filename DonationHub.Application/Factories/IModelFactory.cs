using DonationHub.Domain.Entities;
using DonationHub.Domain.Enums;

namespace DonationHub.Application.Factories;

public interface IModelFactory
{
    Category CreateCategory(string nameAr, string nameEn, string descriptionAr, string descriptionEn);
    Campaign CreateCampaign(decimal goalAmount, DateTime endDate, Guid categoryId);
    Donation CreateDonation(decimal amount, CurrencyValues currency, Guid campaignId);
    PaymentAttempt CreatePaymentAttempt(string checkoutSessionId, decimal amount, Guid donationId);
    IdempotencyRecord CreateIdempotencyRecord(string userId, string requestHash, string responseBody, string responseStatusCode);
    ProcessedStripeEvent CreateProcessedStripeEvent(Guid eventId, string type);
    RefreshTokens CreateRefreshToken(string userId, string tokenHash, string? replacedByTokenId, DateTime expiresAt);
}
