using HotDesk.DataTransferObjects;

namespace HotDesk.Models;

public class ReservationViewModel
{
    public List<EmployeeDto> Employees { get; set; }
    public int SelectedEmployeeId { get; set; }

    public DateTime TimeFrom { get; set; }
    public DateTime TimeTo { get; set; }

    public List<WorkplaceDto> Workplaces { get; set; }
    public int SelectedWorkplaceId { get; set; }

    public WorkplaceDto SelectedWorkplace { get; set; }

    public List<EquipmentDto> AvailableEquipment { get; set; }
}