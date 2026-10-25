using Microsoft.AspNetCore.Mvc;
using Test.Models;

namespace Test.Controllers;

public class EventsController : Controller
{
    // Temporary sample data, no database yet
    public static readonly List<EventRecord> events = new()
    {
        new EventRecord
        {
            Id = 1, CustomerId = 1, Name = "Maria's 18th Birthday", Type = EventType.Debut,
            EventDate = DateTime.Today.AddDays(30), Location = "Taguig Function Hall",
            ExpectedGuests = 120, TotalBudget = 150000, Status = EventStatus.Upcoming
        },
        new EventRecord
        {
            Id = 2, CustomerId = 1, Name = "Company Year-End Party", Type = EventType.CorporateEvent,
            EventDate = DateTime.Today.AddDays(60), Location = "BGC Event Center",
            ExpectedGuests = 200, TotalBudget = 300000, Status = EventStatus.Draft
        },
        new EventRecord
        {
            Id = 3, CustomerId = 1, Name = "Lolo's 70th Birthday", Type = EventType.Birthday,
            EventDate = DateTime.Today.AddDays(-45), Location = "Family Home",
            ExpectedGuests = 40, TotalBudget = 50000, Status = EventStatus.Completed
        }
    };

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Create(EventRecord newEvent)
    {
        newEvent.Id = events.Count == 0 ? 1 : events.Max(e => e.Id) + 1;
        newEvent.CustomerId = 1;
        newEvent.Status = EventStatus.Draft;
        newEvent.CreatedAt = DateTime.Now;

        events.Add(newEvent);
        return RedirectToAction("Index");
    }

        public IActionResult Details(int id)
    {
        var eventRecord = events.FirstOrDefault(e => e.Id == id);

        if (eventRecord == null)
            return NotFound();
        eventRecord.UpdateStatus();
        return View(eventRecord);
    }
        [HttpGet]
    public IActionResult Edit(int id)
    {
        var eventRecord = events.FirstOrDefault(e => e.Id == id);

        if (eventRecord == null)
            return NotFound();

        return View(eventRecord);
    }

    [HttpPost]
    public IActionResult Edit(EventRecord updated)
    {
        var eventRecord = events.FirstOrDefault(e => e.Id == updated.Id);

        if (eventRecord == null)
            return NotFound();

        eventRecord.Name = updated.Name;
        eventRecord.Type = updated.Type;
        eventRecord.EventDate = updated.EventDate;
        eventRecord.StartTime = updated.StartTime;
        eventRecord.EndTime = updated.EndTime;
        eventRecord.Location = updated.Location;
        eventRecord.TotalBudget = updated.TotalBudget;
        eventRecord.ExpectedGuests = updated.ExpectedGuests;
        eventRecord.Description = updated.Description;
        eventRecord.SpecialRequirements = updated.SpecialRequirements;

        return RedirectToAction("Details", new { id = eventRecord.Id });
    }

    [HttpGet]
    public IActionResult Cancel(int id)
    {
        var eventRecord = events.FirstOrDefault(e => e.Id == id);

        if (eventRecord == null)
            return NotFound();

        return View(eventRecord);
    }

    [HttpPost]
    public IActionResult Cancel(int id, string reason)
    {
        var eventRecord = events.FirstOrDefault(e => e.Id == id);

        if (eventRecord == null)
            return NotFound();

        eventRecord.Cancel(reason);
        return RedirectToAction("Details", new { id = eventRecord.Id });
    }
        [HttpGet]
    public IActionResult AddPayment(int id)
    {
        var eventRecord = events.FirstOrDefault(e => e.Id == id);

        if (eventRecord == null)
            return NotFound();

        if (!eventRecord.HasConfirmedBookings)
        {
            TempData["Error"] = "You can add payments once a supplier accepts a booking.";
            return RedirectToAction("Details", new { id });
        }

        return View(eventRecord);
    }

    [HttpPost]
    public IActionResult AddPayment(int eventId, decimal amount, bool isDownPayment, DateTime dueDate, bool markPaid)
    {
        var eventRecord = events.FirstOrDefault(e => e.Id == eventId);

        if (eventRecord == null)
            return NotFound();

        if (!eventRecord.HasConfirmedBookings)
        {
            TempData["Error"] = "You can add payments once a supplier accepts a booking.";
            return RedirectToAction("Details", new { id = eventId });
        }

        if (amount > eventRecord.OutstandingBalance)
        {
            TempData["Error"] = "Payment can't be more than the outstanding balance (₱" + eventRecord.OutstandingBalance.ToString("N2") + ").";
            return RedirectToAction("Details", new { id = eventId });
        }

        var allPayments = events.SelectMany(e => e.Payments).ToList();
        var newId = allPayments.Count == 0 ? 1 : allPayments.Max(p => p.Id) + 1;

        var payment = new EventPayment
        {
            Id = newId,
            EventId = eventId,
            Amount = amount,
            IsDownPayment = isDownPayment,
            DueDate = dueDate
        };

        if (markPaid)
        {
            payment.PaidDate = DateTime.Today;
            payment.ReceiptNumber = "RCPT-" + newId.ToString("0000");
        }

        eventRecord.Payments.Add(payment);
        return RedirectToAction("Details", new { id = eventId });
    }
        public IActionResult Index(string search)
    {
        foreach (var e in events)
            e.UpdateStatus();

        var result = events;

        if (!string.IsNullOrEmpty(search))
            result = events.Where(e => e.Name.Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();

        return View(result);
    }
}