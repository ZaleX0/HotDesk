using HotDesk.DataTransferObjects;
using HotDesk.Models;

namespace HotDesk.Services;

public interface IEquipmentService
{
    EquipmentForWorkplaceViewModel GetEquipmentForWorkplaceViewModel();
    void AddEquipmentToWorkplace(AddEquipmentForWorkplaceDto dto);
}