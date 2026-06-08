using Microsoft.EntityFrameworkCore;

namespace CarRental.Features.Rentals;

public class CarRentalDbContext: DbContext
{
    public CarRentalDbContext(DbContextOptions<CarRentalDbContext> options) : base(options) {
        this.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
    }
    public DbSet<RentalRecord> Rentals => Set<RentalRecord>();
}
