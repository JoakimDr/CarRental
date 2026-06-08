namespace CarRental.Features.Rentals.Pricing
{

    public interface IPriceCalculationStrategy
    {
        CarCategory Category { get; }
        decimal CalculatePrice(DateTime pickupTime, DateTime returnTime, int mileageAtPickup, int mileageAtReturn, decimal baseDayRental, decimal baseKmPrice);
    }

    public class SmallCarPriceStrategy : IPriceCalculationStrategy
    {
        public CarCategory Category => CarCategory.Small;

        public decimal CalculatePrice(DateTime pickupTime, DateTime returnTime, int mileageAtPickup, int mileageAtReturn, decimal baseDayRental, decimal baseKmPrice)
        {
            var rentalDays = (returnTime.Date - pickupTime.Date).Days + 1;
            return baseDayRental * rentalDays;
        }
    }

    public class CombiPriceStrategy : IPriceCalculationStrategy
    {
        public CarCategory Category => CarCategory.Combi;

        public decimal CalculatePrice(DateTime pickupTime, DateTime returnTime, int mileageAtPickup, int mileageAtReturn, decimal baseDayRental, decimal baseKmPrice)
        {
            var rentalDays = (returnTime.Date - pickupTime.Date).Days + 1;
            var rentalKms = mileageAtReturn - mileageAtPickup;

            return (baseDayRental * rentalDays * 1.3m)
                + (baseKmPrice * rentalKms);
        }
    }

    public class TruckPriceStrategy : IPriceCalculationStrategy
    {
        public CarCategory Category => CarCategory.Truck;

        public decimal CalculatePrice(DateTime pickupTime, DateTime returnTime, int mileageAtPickup, int mileageAtReturn, decimal baseDayRental, decimal baseKmPrice)
        {
            var rentalDays = (returnTime.Date - pickupTime.Date).Days + 1;
            var rentalKms = mileageAtReturn - mileageAtPickup;

            return (baseDayRental * rentalDays * 1.5m)
                + (baseKmPrice * rentalKms * 1.5m);
        }
    }
}
