namespace Test.Models;

public class EventRecord
{
    public int Id { get; set; }
    public int CustomerId { get; set; }

    public string Name { get; set; } = "";
    public EventType Type { get; set; } = EventType.Other;
    public DateTime EventDate { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public string Location { get; set; } = "";
    public int ExpectedGuests { get; set; }
    public string Description { get; set; } = "";
    public string SpecialRequirements { get; set; } = "";

    public decimal TotalBudget { get; set; }
    public EventStatus Status { get; set; } = EventStatus.Draft;

    public string CancellationReason { get; set; } = "";
    public DateTime? CancelledAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public List<SupplierBooking> Bookings { get; set; } = new();
    public List<EventPayment> Payments { get; set; } = new();

    // ---- Budget summary ----
    public decimal TotalBookedCost => Bookings
        .Where(b => b.Status == BookingStatus.Accepted)
        .Sum(b => b.Price);

    public decimal RemainingBudget => TotalBudget - TotalBookedCost;

    public double BudgetUsedPercent => TotalBudget <= 0 ? 0 : (double)(TotalBookedCost / TotalBudget * 100);

    public bool IsOverBudget => TotalBookedCost > TotalBudget;

    // ---- Payment summary ----
    public decimal TotalPaid => Payments.Where(p => p.IsPaid).Sum(p => p.Amount);

    public decimal OutstandingBalance => TotalBookedCost - TotalPaid;

    // ---- Rules ----
    public bool HasConfirmedBookings => Bookings.Any(b => b.Status == BookingStatus.Accepted);

    // Changing date/time/location with confirmed bookings needs an availability check + supplier approval
    public bool RequiresSupplierApprovalOnChange => HasConfirmedBookings;
    
    public void UpdateStatus()
    {
        if (Status == EventStatus.Cancelled)
            return;

        // Stays Draft until a supplier accepts a booking
        if (Status == EventStatus.Draft && !HasConfirmedBookings)
            return;

        var start = EventDate.Date + StartTime;
        var end = EventDate.Date + EndTime;

        // Events that run past midnight
        if (end <= start)
            end = end.AddDays(1);

        if (DateTime.Now >= end)
            Status = EventStatus.Completed;
        else if (DateTime.Now >= start)
            Status = EventStatus.Ongoing;
        else
            Status = EventStatus.Upcoming;
    }

    public void Cancel(string reason = "")
    {
        Status = EventStatus.Cancelled;
        CancellationReason = reason;
        CancelledAt = DateTime.Now;

        foreach (var b in Bookings.Where(b => b.Status == BookingStatus.Pending || b.Status == BookingStatus.Accepted))
            b.Status = BookingStatus.Cancelled;
    }
}