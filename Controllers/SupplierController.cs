using Microsoft.AspNetCore.Mvc;
using Test.Models;

namespace Test.Controllers;

public class SupplierController : Controller
{
    // supplierId lets you switch between the sample suppliers while testing
    public IActionResult Index(int supplierId = 1)
    {
        var supplier = CatalogController.suppliers.FirstOrDefault(s => s.Id == supplierId);

        if (supplier == null)
            return NotFound();

        var bookings = EventsController.events
            .SelectMany(e => e.Bookings)
            .Where(b => b.SupplierId == supplierId)
            .OrderBy(b => b.BookingDate)
            .ToList();

        ViewBag.Supplier = supplier;
        return View(bookings);
    }

    [HttpPost]
    public IActionResult Accept(int bookingId, int supplierId)
    {
        var booking = EventsController.events
            .SelectMany(e => e.Bookings)
            .FirstOrDefault(b => b.Id == bookingId && b.SupplierId == supplierId);

        if (booking == null)
            return NotFound();

        if (booking.Status != BookingStatus.Pending)
            return RedirectToAction("Index", new { supplierId });

        // A supplier can't be accepted for two events on the same date
        var dateTaken = EventsController.events
            .SelectMany(e => e.Bookings)
            .Any(b => b.SupplierId == supplierId
                && b.Id != booking.Id
                && b.BookingDate.Date == booking.BookingDate.Date
                && b.Status == BookingStatus.Accepted);

        if (dateTaken)
        {
            TempData["Error"] = "You already have an accepted booking on " + booking.BookingDate.ToString("MMM dd, yyyy") + ".";
            return RedirectToAction("Index", new { supplierId });
        }

        booking.Status = BookingStatus.Accepted;
        return RedirectToAction("Index", new { supplierId });
    }

    [HttpPost]
    public IActionResult Decline(int bookingId, int supplierId)
    {
        var booking = EventsController.events
            .SelectMany(e => e.Bookings)
            .FirstOrDefault(b => b.Id == bookingId && b.SupplierId == supplierId);

        if (booking == null)
            return NotFound();

        if (booking.Status == BookingStatus.Pending)
            booking.Status = BookingStatus.Rejected;

        return RedirectToAction("Index", new { supplierId });
    }
}