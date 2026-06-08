using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using CarRental.Features.Rentals;
using CarRental.Features.Rentals.Pricing;
using CarRental.Features.Rentals.Validation;

using System;
using System.Collections.Generic;
using System.Text;

namespace CarRental.Tests
{
    [TestFixture]
    public class RentalServiceTests
    {
        private CarRentalDbContext _context;
        private RentalService _service;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<CarRentalDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new CarRentalDbContext(options);

            var strategies = new IPriceCalculationStrategy[]
                {
                    new SmallCarPriceStrategy(),
                    new CombiPriceStrategy(),
                    new TruckPriceStrategy()
            };

            var testPricing = new RentalPricingOptions
            {
                BaseDayRental = 300.0m,
                BaseKmPrice = 2.5m
            };

            var optionsMock = Microsoft.Extensions.Options.Options.Create(testPricing);

            var priceCalcService = new PriceCalculationService(strategies, optionsMock);
            var validator = new PickupRequestValidator();

            _service = new RentalService(_context, priceCalcService, validator, Microsoft.Extensions.Logging.Abstractions.NullLogger<RentalService>.Instance);
        }

        [TearDown]
        public void TearDown() => _context.Dispose();

        [Test]
        public async Task Pickup_Should_Save_Rental_To_Database()
        {
            // Arrange
            var request = new PickupRequest("99", "ABC123", "19700101-1234", CarCategory.Small, DateTime.UtcNow, 1000);

            // Act
            var result = await _service.ProcessPickupAsync(request);

            // Assert
            result.BookingNumber.Should().Be("99");
            _context.Rentals.Count().Should().Be(1);
        }

        [Test]
        public async Task Return_With_Lower_Mileage_Should_Throw_ValidationException()
        {
            // Arrange
            var active = new RentalRecord
            {
                BookingNumber = "99",
                CarRegistrationNumber = "ABC123",
                CustomerSSN = "19700101-1234",
                CarCategory = CarCategory.Small,
                PickupTime = DateTime.UtcNow.AddDays(-2),
                MileagePickup = 50000
            };

            _context.Rentals.Add(active);
            await _context.SaveChangesAsync();

            var fakeRequest = new ReturnRequest("99", DateTime.UtcNow, 49999);

            //Act
            Func<Task> act = async () => await _service.ProcessReturnAsync(fakeRequest);
            await act.Should().ThrowAsync<FluentValidation.ValidationException>();
        }
    }
}
