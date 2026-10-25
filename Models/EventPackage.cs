namespace Test.Models;

// A package an event organizer builds from their partner suppliers' services
public class EventPackage
{
    public int Id { get; set; }
    public int OrganizerId { get; set; }

    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public decimal Price { get; set; }

    // Ids of the SupplierServices included in this package
    public List<int> ServiceIds { get; set; } = new();

    // Clients only see the package once it's published
    public bool IsPublished { get; set; }
}