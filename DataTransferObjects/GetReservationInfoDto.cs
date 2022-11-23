namespace HotDesk.DataTransferObjects;

public class GetReservationInfoDto
{
    public int EmployeeId { get; set; }
    public DateTime From { get; set; }
    public DateTime To { get; set; }
    public int WorkplaceId { get; set; }
}
