
using DonationHub.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace DonationHub.Domain.Entities
{
    public class Money : ValueObject
    {
        [Range(1, 10000), Required(ErrorMessage = "Amount field is required")]
        public decimal Amount { get; }
        public CurrencyValues currency { get; } = CurrencyValues.USD;
        public Money(decimal amount, CurrencyValues currency)
        {
            if (amount <= 0)
                throw new ArgumentException("amount must be greater than 0");

            Amount = amount;
            this.currency = currency;
        }
        public override IEnumerable<object> GetAtomicValues()
        {
            yield return Amount;
            yield return currency;
        }
    }
}
