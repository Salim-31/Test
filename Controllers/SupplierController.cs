using Microsoft.AspNetCore.Mvc;
using Test.Models;

namespace Test.Controllers;

public class SupplierController : Controller
{
    public IActionResult Index(int supplierId = 1)
    {
        var supplier = CatalogController.suppliers
            .FirstOrDefault(s => s.Id == supplierId);

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
            .FirstOrDefault(b =>
                b.Id == bookingId &&
                b.SupplierId == supplierId);

        if (booking == null)
            return NotFound();

        if (booking.Status != BookingStatus.Pending)
            return RedirectToAction("Index", new { supplierId });

        var dateTaken = EventsController.events
            .SelectMany(e => e.Bookings)
            .Any(b =>
                b.SupplierId == supplierId &&
                b.Id != booking.Id &&
                b.BookingDate.Date == booking.BookingDate.Date &&
                b.Status == BookingStatus.Accepted);

        if (dateTaken)
        {
            TempData["Error"] =
                "You already have an accepted booking on " +
                booking.BookingDate.ToString("MMM dd, yyyy") + ".";

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
            .FirstOrDefault(b =>
                b.Id == bookingId &&
                b.SupplierId == supplierId);

        if (booking == null)
            return NotFound();

        if (booking.Status == BookingStatus.Pending)
            booking.Status = BookingStatus.Rejected;

        return RedirectToAction("Index", new { supplierId });
    }

    public IActionResult Services(int supplierId = 1)
    {
        var supplier = CatalogController.suppliers
            .FirstOrDefault(s => s.Id == supplierId);

        if (supplier == null)
            return NotFound();

        ViewBag.Supplier = supplier;

        var services = CatalogController.services
            .Where(s => s.SupplierId == supplierId && s.IsActive)
            .ToList();

        return View(services);
    }

    public IActionResult AddService(int supplierId = 1)
    {
        var supplier = CatalogController.suppliers
            .FirstOrDefault(s => s.Id == supplierId);

        if (supplier == null)
            return NotFound();

        ViewBag.SupplierId = supplierId;

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult AddService(SupplierService service)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.SupplierId = service.SupplierId;
            return View(service);
        }

        service.Id = CatalogController.services.Count == 0
            ? 1
            : CatalogController.services.Max(s => s.Id) + 1;

        service.IsActive = true;

        CatalogController.services.Add(service);

        return RedirectToAction(
            "Services",
            new { supplierId = service.SupplierId });
    }

    public IActionResult EditService(int id, int supplierId = 1)
    {
        var service = CatalogController.services
            .FirstOrDefault(s =>
                s.Id == id &&
                s.SupplierId == supplierId &&
                s.IsActive);

        if (service == null)
            return NotFound();

        ViewBag.SupplierId = supplierId;

        return View(service);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult EditService(SupplierService updatedService)
    {
        var service = CatalogController.services
            .FirstOrDefault(s =>
                s.Id == updatedService.Id &&
                s.SupplierId == updatedService.SupplierId &&
                s.IsActive);

        if (service == null)
            return NotFound();

        if (!ModelState.IsValid)
        {
            ViewBag.SupplierId = updatedService.SupplierId;
            return View(updatedService);
        }

        service.Name = updatedService.Name;
        service.Description = updatedService.Description;
        service.Price = updatedService.Price;

        return RedirectToAction(
            "Services",
            new { supplierId = service.SupplierId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteService(int id, int supplierId)
    {
        var service = CatalogController.services
            .FirstOrDefault(s =>
                s.Id == id &&
                s.SupplierId == supplierId &&
                s.IsActive);

        if (service == null)
            return NotFound();

        service.IsActive = false;

        return RedirectToAction(
            "Services",
            new { supplierId });
    }
}