using CarRental.Features.Rentals.Pricing;
using CarRental.Features.Rentals.Validation;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Serilog.Core;

namespace CarRental.Features.Rentals;

public class RentalService(CarRentalDbContext context, PriceCalculationService priceService, IValidator<PickupRequest> pickupValidator, ILogger<RentalService> logger)
{
    public async Task<RentalRecord> ProcessPickupAsync(PickupRequest request, string nationalIdFromJwt)
    {
        var validationResult = await pickupValidator.ValidateAsync(request);
        if (!validationResult.IsValid){
            logger.LogWarning("Pickup-validering misslyckades för bokning {bookingnumber}", request.BookingNumber);
            throw new ValidationException(validationResult.Errors);
        }

        var exists = await context.Rentals.AnyAsync(r => r.BookingNumber == request.BookingNumber && r.Status == RentalStatus.Active);

        if (exists)
        {
            logger.LogWarning("Pickup misslyckades. Bokning {bookingnumber} är redan aktiv", request.BookingNumber);
            throw new InvalidOperationException("Bokningen är redan aktiv.");
        }

        var rental = new RentalRecord
        {
            BookingNumber = request.BookingNumber,
            CarRegistrationNumber = request.CarRegistrationNumber,
            CustomerSSN = nationalIdFromJwt,
            CarCategory = request.CarCategory,
            PickupTime = request.PickupTime,
            MileagePickup = request.CurrentMeterReading
        };

        context.Rentals.Add(rental);
        await context.SaveChangesAsync();

        logger.LogInformation("Bil uthämtad. Bokning {BookingNumber}, Bil: {RegNumber}.", rental.BookingNumber, rental.CarRegistrationNumber);

        return rental;

    }

    public async Task<decimal> ProcessReturnAsync(ReturnRequest request, string nationalIdFromJwt)
    {
        var rental = await context.Rentals.FirstOrDefaultAsync(r => r.BookingNumber == request.BookingNumber && r.Status == RentalStatus.Active);
        if(rental == null)
        {
            logger.LogError("Återlämning misslyckades. Aktiv bokning {bookingnumber} hittades inte i databasen", request.BookingNumber);
            throw new KeyNotFoundException("Bokningen hittades inte");
        }

        if(rental.CustomerSSN != nationalIdFromJwt)
        {
            logger.LogWarning("Säkerhetsöverträdelse. Användare med personnummer {jwtId} försökte återlämna bil bokad av {rentalRecordId}", nationalIdFromJwt, rental.CustomerSSN);
            throw new UnauthorizedAccessException("Du kan endast återlämna bilar som du själv har bokat.");
        }

        var validator = new ReturnRequestValidator(rental);
        var valResult = await validator.ValidateAsync(request);
        if (!valResult.IsValid)
        {
            logger.LogWarning("Återlämnningsvalidering misslyckades för bokning {bookingnumber}", request.BookingNumber);
            throw new ValidationException(valResult.Errors);
        }

        decimal price = priceService.Calculate(rental.CarCategory, rental.PickupTime, request.ReturnTime, rental.MileagePickup, request.CurrentMeterReading);

        rental.ReturnTime = request.ReturnTime;
        rental.MileageReturn = request.CurrentMeterReading;
        rental.CalculatedPrice = price;
        rental.Status = RentalStatus.Completed;

        await context.SaveChangesAsync();

        logger.LogInformation("Bil återlämnad. Bokning {BookingNumber}, slutpris: {price}.", rental.BookingNumber, price);
        return price;

    }
}
