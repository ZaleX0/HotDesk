namespace HotDesk.DataTransferObjects;

public class WorkplaceDto
{
    public int Id { get; set; }
    public int Floor { get; set; }
    public int Room { get; set; }
    public int Table { get; set; }

    public List<EquipmentForWorkplaceDto>? Equipment { get; set; }

    public bool IsReserved { get; set; }
    public bool IsReservedInFuture { get; set; }
}
