using FluentAssertions;
using CarRental.Features.Rentals;
using CarRental.Features.Rentals.Pricing;
using NUnit.Framework;

using System;
using System.Collections.Generic;
using System.Text;

namespace CarRental.Tests;

[TestFixture]
public class PricingTests
{
    private readonly DateTime _pickupDate = new DateTime(2026,6,1,10,0,0);
    private readonly DateTime _returnThreeDaysLater = new DateTime(2026,6,4,10,0,0);

    private const decimal BaseDayRental = 300.0m;
    private const decimal BaseKmPrice = 2.5m;

    [Test]
    public void SmallCar_Should_Only_Charge_forDays()
    {
        var strategy = new SmallCarPriceStrategy();
        decimal expected = BaseDayRental * 4;
        decimal result = strategy.CalculatePrice(_pickupDate, _returnThreeDaysLater, 1000, 1500, BaseDayRental, BaseKmPrice);

        result.Should().Be(expected);
    }

    [Test]
    public void SmallCar_Should_Apply_Formulas_With_Correct_Factors()
    {
        var strategy = new CombiPriceStrategy();
        int mileagePickup = 10000;
        int mileageReturn = 10200;

        decimal expected = (BaseDayRental * 4 * 1.3m) + (BaseKmPrice * 200);
        decimal result = strategy.CalculatePrice(_pickupDate, _returnThreeDaysLater, mileagePickup, mileageReturn, BaseDayRental, BaseKmPrice);

        result.Should().Be(expected);

    }

    [Test]
    public void Truck_Should_Apply_Formulas_With_Correct_Factors()
    {
        var strategy = new TruckPriceStrategy();
        int mileagePickup = 20000;
        int mileageReturn = 20100;

        decimal expected = (BaseDayRental * 4 * 1.5m) + (BaseKmPrice * 100 * 1.5m);
        decimal result = strategy.CalculatePrice(_pickupDate, _returnThreeDaysLater, mileagePickup, mileageReturn, BaseDayRental, BaseKmPrice);

        result.Should().Be(expected);

    }


}
