using FluentAssertions;
using CarRental.Features.Rentals;
using CarRental.Features.Rentals.Validation;

using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace CarRental.Tests;

[TestFixture]
public class ValidationTests
{
    private PickupRequestValidator _pickupValidator;

    [SetUp]
    public void Setup()
    {
        _pickupValidator = new PickupRequestValidator();
    }

    [Test]
    public async Task Pickup_WityhInvalid_Registration_Number_Length_Should_Fail()
    {
        var request = new PickupRequest
        (
            "BOK-1",
            "ABC1234",
            CarCategory.Small,
            DateTime.UtcNow,
            1000
        );

        var result = await _pickupValidator.ValidateAsync(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == nameof(PickupRequest.CarRegistrationNumber));
          
    }

    [Test]
    public async Task ReturnMileage_Less_than_PickupMileage_Should_Fail_Validation()
    {
        var existingRental = new RentalRecord
        {
            BookingNumber = "2",
            CarRegistrationNumber = "ABC123",
            CustomerSSN = "19700501-1234",
            CarCategory = CarCategory.Combi,
            PickupTime = DateTime.UtcNow.AddDays(-3),
            MileagePickup = 50000
        };

        var invalidRequest = new ReturnRequest("2", DateTime.UtcNow, 49999);
        var validator = new ReturnRequestValidator(existingRental);

        var result = await validator.ValidateAsync(invalidRequest);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle()
            .Which.ErrorMessage.Should().Contain("kan inte vara lägre än vid uthämtning");
    }
}
