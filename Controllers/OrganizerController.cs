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
}