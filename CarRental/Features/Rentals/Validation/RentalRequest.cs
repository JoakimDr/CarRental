namespace CarRental.Features.Rentals.Validation
{
    public record PickupRequest(
        string BookingNumber,
        string CarRegistrationNumber,
        string CustomerSSN,
        CarCategory CarCategory,
        DateTime PickupTime,
        int CurrentMeterReading
    );

    public record ReturnRequest(
        string BookingNumber,
        DateTime ReturnTime,
        int CurrentMeterReading
    );

    public record ReturnResponse(
        string Message,
        decimal Price
    );
}
