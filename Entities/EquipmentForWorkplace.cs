namespace HotDesk.Entities;

public class EquipmentForWorkplace
{
    public int Id { get; set; }
    public int WorkplaceId { get; set; }
    public int EquipmentId { get; set; }
    public int Count { get; set; }

    public virtual Equipment Equipment { get; set; }
    public virtual Workplace Workplace { get; set; }
}
