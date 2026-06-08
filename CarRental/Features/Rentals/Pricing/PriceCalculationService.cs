using Microsoft.Extensions.Options;

namespace CarRental.Features.Rentals.Pricing;

public class PriceCalculationService(IEnumerable<IPriceCalculationStrategy> strategies, IOptions<RentalPricingOptions> pricingOptions)
{
    private readonly Dictionary<CarCategory, IPriceCalculationStrategy> _strategies = strategies.ToDictionary(s => s.Category);
    private readonly RentalPricingOptions _options = pricingOptions.Value;

    public decimal Calculate(CarCategory category, DateTime pickupTime, DateTime returnTime, int mileagePickup, int mileageReturn)
    {

        if(!_strategies.TryGetValue(category, out var strategy))
        {
            throw new NotSupportedException($"Prisstrategi för bilkategorin '{category}' saknas i systemet");
        }

        return strategy.CalculatePrice(pickupTime, returnTime, mileagePickup, mileageReturn, _options.BaseDayRental, _options.BaseKmPrice);
    }
}
