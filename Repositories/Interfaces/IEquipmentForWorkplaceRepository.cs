using HotDesk.Entities;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace HotDesk.Repositories.Interfaces;
public interface IEquipmentForWorkplaceRepository
{
    void Add(EquipmentForWorkplace equipmentForWorkplace);
    void AddRange(IEnumerable<EquipmentForWorkplace> equipmentForWorkplaces);
    void ClearChangeTracker();
    IEnumerable<EquipmentForWorkplace> GetAll();
    IEnumerable<EntityEntry> GetChangeTrackerEntries();
    IEnumerable<EquipmentForWorkplace> GetForWorkplace(int workplaceId);
    void RemoveRange(IEnumerable<EquipmentForWorkplace> equipmentForWorkplaces);
    void SaveChanges();
    void UpdateRange(IEnumerable<EquipmentForWorkplace> equipmentForWorkplaces);
}