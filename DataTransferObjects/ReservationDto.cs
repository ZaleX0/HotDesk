namespace HotDesk.DataTransferObjects;

public class ReservationDto
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public string EmployeeName { get; set; }
    public int WorkplaceId { get; set; }
    public int Floor { get; set; }
    public int Room { get; set; }
    public int Table { get; set; }
    public DateTime TimeFrom { get; set; }
    public DateTime TimeTo { get; set; }
}
