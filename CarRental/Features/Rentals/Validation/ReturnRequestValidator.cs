using FluentValidation;

namespace CarRental.Features.Rentals.Validation
{
    public class ReturnRequestValidator : AbstractValidator<ReturnRequest>
    {
        public ReturnRequestValidator(RentalRecord existingRental)
        {
            RuleFor(p => p.BookingNumber)
                .NotEmpty().WithMessage("Var god ange bokningsnummer.");

            RuleFor(p => p.ReturnTime)
                .GreaterThanOrEqualTo(existingRental.PickupTime)
                .WithMessage("Återlämningstiden kan inte vara tidigare än uthämtningstiden.");

            RuleFor(p => p.CurrentMeterReading)
                .GreaterThanOrEqualTo(existingRental.MileagePickup)
                .WithMessage($"Mätarställningen vid återlämning kan inte vara lägre än vid uthämtning.");
        }
    }
}
