namespace Test.Models;

public class SupplierService
{
    public int Id { get; set; }
    public int SupplierId { get; set; }

    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public decimal Price { get; set; }
    public bool IsActive { get; set; } = true;
}