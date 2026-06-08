using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarRental.Features.Rentals
{
    public class RentalModels
    {
    }

    public enum CarCategory
    {
        Small,
        Combi,
        Truck
    }

    public enum RentalStatus
    {
        Active,
        Completed
    }

    public class RentalRecord
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RentalId { get; set; }
        public required string BookingNumber { get; init; }
        public required string CarRegistrationNumber {  get; init; }
        public required string CustomerSSN { get; init; }
        public required CarCategory CarCategory { get; init; }
        public required DateTime PickupTime { get; init; }
        public required int MileagePickup {  get; init; }
        public  DateTime? ReturnTime { get; set; }
        public  int? MileageReturn { get; set; }
        public decimal? CalculatedPrice { get; set; }
        public RentalStatus Status { get; set; } = RentalStatus.Active;
    }
}
