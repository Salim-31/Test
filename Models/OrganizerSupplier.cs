namespace Test.Models;

// Records that a supplier partners with an organizer,
// so the supplier's services can be offered through that organizer
public class OrganizerSupplier
{
    public int Id { get; set; }
    public int OrganizerId { get; set; }
    public int SupplierId { get; set; }
}