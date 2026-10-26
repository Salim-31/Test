using Microsoft.AspNetCore.Mvc;
using Test.Models;

namespace Test.Controllers;

public class OrganizerController : Controller
{
    // Temporary sample data, no database yet
    public static readonly List<Organizer> organizers = new()
    {
        new Organizer { Id = 1, Name = "Grand Events Co.", Email = "grand@example.com", Phone = "0917-100-0001" },
        new Organizer { Id = 2, Name = "Celebrate PH", Email = "celebrate@example.com", Phone = "0917-100-0002" }
    };

    public static readonly List<OrganizerSupplier> partnerships = new()
    {
        new OrganizerSupplier { Id = 1, OrganizerId = 1, SupplierId = 1 },
        new OrganizerSupplier { Id = 2, OrganizerId = 1, SupplierId = 2 },
        new OrganizerSupplier { Id = 3, OrganizerId = 1, SupplierId = 3 },
        new OrganizerSupplier { Id = 4, OrganizerId = 2, SupplierId = 2 },
        new OrganizerSupplier { Id = 5, OrganizerId = 2, SupplierId = 3 }
    };

    // organizerId lets you switch between the sample organizers while testing
    public IActionResult Index(int organizerId = 1)
    {
        var organizer = organizers.FirstOrDefault(o => o.Id == organizerId);

        if (organizer == null)
            return NotFound();

        var partnerIds = partnerships
            .Where(p => p.OrganizerId == organizerId)
            .Select(p => p.SupplierId)
            .ToList();

        ViewBag.Organizer = organizer;
        ViewBag.Suppliers = CatalogController.suppliers.Where(s => partnerIds.Contains(s.Id)).ToList();
        ViewBag.Services = CatalogController.services
            .Where(s => s.IsActive && partnerIds.Contains(s.SupplierId))
            .ToList();

        return View();
    }
        public IActionResult Packages(int organizerId = 1)
    {
        var organizer = organizers.FirstOrDefault(o => o.Id == organizerId);

        if (organizer == null)
            return NotFound();

        ViewBag.Organizer = organizer;
        ViewBag.Services = CatalogController.services;

        var myPackages = CatalogController.packages
            .Where(p => p.OrganizerId == organizerId)
            .ToList();

        return View(myPackages);
    }

    [HttpPost]
    public IActionResult TogglePublish(int packageId, int organizerId)
    {
        var package = CatalogController.packages
            .FirstOrDefault(p => p.Id == packageId && p.OrganizerId == organizerId);

        if (package == null)
            return NotFound();

        package.IsPublished = !package.IsPublished;
        return RedirectToAction("Packages", new { organizerId });
    }
        [HttpGet]
    public IActionResult CreatePackage(int organizerId = 1)
    {
        var organizer = organizers.FirstOrDefault(o => o.Id == organizerId);

        if (organizer == null)
            return NotFound();

        // Only services from this organizer's partner suppliers
        var partnerIds = partnerships
            .Where(p => p.OrganizerId == organizerId)
            .Select(p => p.SupplierId)
            .ToList();

        ViewBag.Organizer = organizer;
        ViewBag.Suppliers = CatalogController.suppliers;
        ViewBag.Services = CatalogController.services
            .Where(s => s.IsActive && partnerIds.Contains(s.SupplierId))
            .ToList();

        return View();
    }

        [HttpPost]
    public IActionResult CreatePackage(int organizerId, string name, string description, List<int> serviceIds)
    {
        if (serviceIds == null || serviceIds.Count == 0)
        {
            TempData["Error"] = "Pick at least one service for the package.";
            return RedirectToAction("CreatePackage", new { organizerId });
        }

        // Package price = total of the chosen services
        var total = CatalogController.services
            .Where(s => serviceIds.Contains(s.Id))
            .Sum(s => s.Price);

        var package = new EventPackage
        {
            Id = CatalogController.packages.Count == 0 ? 1 : CatalogController.packages.Max(p => p.Id) + 1,
            OrganizerId = organizerId,
            Name = name,
            Description = description ?? "",
            Price = total,
            ServiceIds = serviceIds,
            IsPublished = false
        };

        CatalogController.packages.Add(package);
        return RedirectToAction("Packages", new { organizerId });
    }
}