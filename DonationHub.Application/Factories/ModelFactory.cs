using DonationHub.Domain.Entities;
using DonationHub.Domain.Enums;

namespace DonationHub.Application.Factories;

public sealed class ModelFactory(TimeProvider timeProvider) : IModelFactory
{
    //. still does not depending on the abstraction 
    public Category CreateCategory(string nameAr, string nameEn, string descriptionAr, string descriptionEn)
    {
        var now = timeProvider.GetUtcNow().UtcDateTime;
        return new Category
        {
            NameAr = RequireValue(nameAr, nameof(nameAr)),
            NameEn = RequireValue(nameEn, nameof(nameEn)),
            DescriptionAr = RequireValue(descriptionAr, nameof(descriptionAr)),
            DescriptionEn = RequireValue(descriptionEn, nameof(descriptionEn)),
            CreatedAt = now,
            UpdatedAt = now
        };
    }

    public Campaign CreateCampaign(decimal goalAmount, DateTime endDate, Guid categoryId)
    {
        var now = timeProvider.GetUtcNow().UtcDateTime;
        return new Campaign(goalAmount, endDate, now)
        {
            CategoryId = RequireId(categoryId, nameof(categoryId)),
            CreatedAt = now,
            UpdatedAt = now
        };
    }

    public Donation CreateDonation(decimal amount, CurrencyValues currency, Guid campaignId)
    {
        var now = timeProvider.GetUtcNow().UtcDateTime;
        return new Donation(amount, currency)
        {
            CampaignId = RequireId(campaignId, nameof(campaignId)),
            CreatedAt = now,
            UpdatedAt = now
        };
    }

    public PaymentAttempt CreatePaymentAttempt(string checkoutSessionId, decimal amount, Guid donationId)
    {
        var now = timeProvider.GetUtcNow().UtcDateTime;
        return new PaymentAttempt
        {
            StripeCheckoutSessionId = RequireValue(checkoutSessionId, nameof(checkoutSessionId)),
            Amount = amount,
            DonationId = RequireId(donationId, nameof(donationId)),
            Status = PaymentAttemptStatus.Created.ToString(),
            CreatedAt = now,
            UpdatedAt = now
        };
    }

    public IdempotencyRecord CreateIdempotencyRecord(string userId, string requestHash, string responseBody, string responseStatusCode)
    {
        var now = timeProvider.GetUtcNow().UtcDateTime;
        return new IdempotencyRecord
        {
            Key = Guid.NewGuid(),
            UserId = RequireValue(userId, nameof(userId)),
            RequestHash = RequireValue(requestHash, nameof(requestHash)),
            ResponseBody = RequireValue(responseBody, nameof(responseBody)),
            ResponseStatusCode = RequireValue(responseStatusCode, nameof(responseStatusCode)),
            CreatedAt = now,
            UpdatedAt = now
        };
    }

    public ProcessedStripeEvent CreateProcessedStripeEvent(Guid eventId, string type)
    {
        return new ProcessedStripeEvent
        {
            EventId = RequireId(eventId, nameof(eventId)),
            Type = RequireValue(type, nameof(type)),
            ProcessedAt = timeProvider.GetUtcNow().UtcDateTime
        };
    }

    public RefreshTokens CreateRefreshToken(string userId, string tokenHash, string? replacedByTokenId, DateTime expiresAt)
    {
        var now = timeProvider.GetUtcNow().UtcDateTime;
        if (expiresAt <= now)
        {
            throw new ArgumentException("The refresh token expiry must be in the future.", nameof(expiresAt));
        }

        return new RefreshTokens
        {
            UserId = RequireValue(userId, nameof(userId)),
            TokenHash = RequireValue(tokenHash, nameof(tokenHash)),
            ReplacedByTokenId = string.IsNullOrWhiteSpace(replacedByTokenId) ? null : replacedByTokenId.Trim(),
            CreatedAt = now,
            ExpiresAt = expiresAt
        };
    }

    private static string RequireValue(string value, string parameterName) =>
        string.IsNullOrWhiteSpace(value)
            ? throw new ArgumentException("A value is required.", parameterName)
            : value.Trim();

    private static Guid RequireId(Guid value, string parameterName) =>
        value == Guid.Empty
            ? throw new ArgumentException("A non-empty identifier is required.", parameterName)
            : value;
}
