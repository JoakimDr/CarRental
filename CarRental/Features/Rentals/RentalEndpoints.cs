using CarRental.Features.Rentals.Pricing;
using CarRental.Features.Rentals.Validation;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace CarRental.Features.Rentals;

public static class RentalEndpoints
{
    public static void MapRentalEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/rentals").RequireAuthorization();

        group.MapPost("/pickup", async (
                PickupRequest request,
                RentalService service
            ) =>
        {
            try
            {
                var result = await service.ProcessPickupAsync(request);
                return Results.Created($"/rentals/{result.BookingNumber}", result);
            }
            catch (ValidationException ex)
            {
                var errors = ex.Errors.GroupBy(e => e.PropertyName)
                    .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());
                return Results.ValidationProblem(errors);
            }

        });

        group.MapPost("/return", async (
                ReturnRequest request,
                RentalService service
            ) =>
        {
            try
            {
                var price = await service.ProcessReturnAsync(request);
                return Results.Ok(new ReturnResponse("Bilen har återlämnats", price));

            }
            catch (ValidationException ex )
            {
                var errors = ex.Errors.GroupBy(e => e.PropertyName)
                    .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());
                return Results.ValidationProblem(errors);
            }

        });
    }
}
