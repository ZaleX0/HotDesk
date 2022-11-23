namespace HotDesk.DataTransferObjects;

public class EquipmentForWorkplaceDto
{
    public int Id { get; set; }
    public int WorkplaceId { get; set; }
    public int EquipmentId { get; set; }
    public string Type { get; set; }
    public int Count { get; set; }
}
