using HotDesk.DataTransferObjects;

namespace HotDesk.Models;

public class EquipmentForWorkplaceViewModel
{
    public List<WorkplaceDto> Workplaces { get; set; }
    public List<EquipmentDto> Equipment { get; set; }
}
