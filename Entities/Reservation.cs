namespace HotDesk.Entities;

public class Reservation
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public int WorkplaceId { get; set; }
    public DateTime TimeFrom { get; set; }
    public DateTime TimeTo { get; set; }
}
