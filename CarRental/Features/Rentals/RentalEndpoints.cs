using CarRental.Features.Rentals.Pricing;
using CarRental.Features.Rentals.Validation;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CarRental.Features.Rentals;

public static class RentalEndpoints
{
    public static void MapRentalEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/rentals").RequireAuthorization();

        group.MapPost("/pickup", async (
                PickupRequest request,
                RentalService service,
                ClaimsPrincipal user
            ) =>
        {
            var nationalIdFromJwt = user.FindFirst("nationalid")?.Value;

            if (string.IsNullOrEmpty(nationalIdFromJwt)) {
                return Results.Forbid();
            }

            try
            {
                var result = await service.ProcessPickupAsync(request, nationalIdFromJwt);
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
                RentalService service,
                ClaimsPrincipal user
            ) =>
        {
            var nationalIdFromJwt = user.FindFirst("nationalid")?.Value;

            if (string.IsNullOrEmpty(nationalIdFromJwt)) {
                return Results.Forbid();
            }

            try
            {
                var price = await service.ProcessReturnAsync(request, nationalIdFromJwt);
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
