namespace Test.Models;

public class SupplierBooking
{
    public int Id { get; set; }
    public int EventId { get; set; }

    public int SupplierId { get; set; }
    public int ServiceId { get; set; }
    public string ServiceName { get; set; } = "";

    // Set only when the service was booked as part of an organizer's package
    public int? PackageId { get; set; }
    public bool IsFromPackage => PackageId.HasValue;

    public decimal Price { get; set; }
    public DateTime BookingDate { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }

    // Pending until the supplier accepts or declines
    public BookingStatus Status { get; set; } = BookingStatus.Pending;
    public string ModificationRequest { get; set; } = "";
}