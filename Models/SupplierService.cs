namespace Test.Models;

// A service a supplier offers, e.g. "Sound system and catering"
public class SupplierService
{
    public int Id { get; set; }
    public int SupplierId { get; set; }

    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public decimal Price { get; set; }
    public bool IsActive { get; set; } = true;
}