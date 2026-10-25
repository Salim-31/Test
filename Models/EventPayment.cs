namespace Test.Models;

public class EventPayment
{
    public int Id { get; set; }
    public int EventId { get; set; }
    public int BookingId { get; set; }

    public decimal Amount { get; set; }
    public bool IsDownPayment { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime? PaidDate { get; set; }
    public string ReceiptNumber { get; set; } = "";

    public bool IsPaid => PaidDate.HasValue;
    public bool IsOverdue => !IsPaid && DueDate.Date < DateTime.Today;
}