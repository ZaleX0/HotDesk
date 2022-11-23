namespace HotDesk.DataTransferObjects;

public class CreateReservationDto
{
    public int EmployeeId { get; set; }
    public int WorkplaceId { get; set; }
    public DateTime From { get; set; }
    public DateTime To { get; set; }
    public List<EquipmentDto> AdditionalEquipment { get; set; }
}
