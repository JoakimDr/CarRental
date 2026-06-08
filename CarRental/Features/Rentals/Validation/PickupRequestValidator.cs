using FluentValidation;

namespace CarRental.Features.Rentals.Validation
{
    public class PickupRequestValidator : AbstractValidator<PickupRequest>
    {
        public PickupRequestValidator() {
            RuleFor(x => x.BookingNumber)
                .NotEmpty().WithMessage("Var god ange ett bokningsnummer.");

            RuleFor(x => x.CarRegistrationNumber)
                .NotEmpty().WithMessage("Var god ange ett registreringsnummer")
                .Length(6).WithMessage("Var god ange ett registreringsnummer med 6 tecken.");

            RuleFor(x => x.CustomerSSN)
                .NotEmpty().WithMessage("Var god ange ditt personnummer.")
                .Matches(@"^(\d{6}|\d{8})-\d{4}$").WithMessage("Var god ange ett personnummer på formatet ÅÅMMDD-XXXX eller ÅÅÅÅMMDD-XXXX.");

            RuleFor(x => x.CarCategory)
                .IsInEnum().WithMessage("Var god ange en giltig bilkategori.");

            RuleFor(x => x.PickupTime)
                .NotEmpty().WithMessage("Var god ange en uthämtningstid.");

            RuleFor(x => x.CurrentMeterReading)
                .GreaterThanOrEqualTo(0).WithMessage("Var god ange en giltig mätarställning.");
        
        }
    }
}
