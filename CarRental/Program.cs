using CarRental.Features.Rentals;
using CarRental.Features.Rentals.Pricing;
using CarRental.Features.Rentals.Validation;
using CarRental.Middleware;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddSerilog((services, loggerConfig) => loggerConfig
.ReadFrom.Configuration(builder.Configuration)
.WriteTo.Console()
.Enrich.FromLogContext());

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddOpenApi( options =>
{
    options.AddDocumentTransformer((document, context, cancellationToken) =>
    {
        document.Info.Description += " | kräver JWT Autentisering (Bearer)";

        //var reqs = new Microsoft.AspNetCore.OpenApi.openapis

        return Task.CompletedTask;
    });
});

// Register EF Core
builder.Services.AddDbContext<CarRentalDbContext>(options =>
    options.UseInMemoryDatabase("CarRentalDb"));

//Bind appsettings pricing to options class
builder.Services.Configure<RentalPricingOptions>(builder.Configuration.GetSection("RentalPricing"));

// Register Price strategies
builder.Services.AddSingleton<IPriceCalculationStrategy, SmallCarPriceStrategy>();
builder.Services.AddSingleton<IPriceCalculationStrategy, CombiPriceStrategy>();
builder.Services.AddSingleton<IPriceCalculationStrategy, TruckPriceStrategy>();
builder.Services.AddSingleton<PriceCalculationService>();

builder.Services.AddScoped<RentalService>();

// Register FluentValidation
builder.Services.AddScoped<IValidator<PickupRequest>, PickupRequestValidator>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer();
builder.Services.AddAuthorization();

var app = builder.Build();

// Register ExceptionHandlingMiddleware
app.UseMiddleware<ExceptionHandlingMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.Authentication = new ScalarAuthenticationOptions
        {
            PreferredSecurityScheme = "Bearer"
        };
    });

}

app.UseAuthentication();
app.UseAuthorization();

app.UseHttpsRedirection();

app.MapGet("/", () => "CarRental api is running");
app.MapRentalEndpoints();
app.Run();
