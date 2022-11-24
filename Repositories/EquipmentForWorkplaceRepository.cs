using HotDesk.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace HotDesk.Repositories;

public class EquipmentForWorkplaceRepository
{
	private readonly HotDeskDbContext _context;

	public EquipmentForWorkplaceRepository(HotDeskDbContext context)
	{
		_context = context;
	}

	public IEnumerable<EquipmentForWorkplace> GetAll()
	{
		return _context.EquipmentForWorkplace
			.Include(x => x.Equipment)
			.Include(x => x.Workplace)
			.ToList();
	}

	public IEnumerable<EquipmentForWorkplace> GetForWorkplace(int workplaceId)
	{
        return _context.EquipmentForWorkplace
            .Where(x => x.WorkplaceId == workplaceId)
            .Include(x => x.Equipment)
            .Include(x => x.Workplace)
            .ToList();
    }

	public void UpdateRange(IEnumerable<EquipmentForWorkplace> equipmentForWorkplaces)
	{
		_context.UpdateRange(equipmentForWorkplaces);

		

		//_context.EquipmentForWorkplace.UpdateRange(equipmentForWorkplaces);
	}

	public void RemoveRange(IEnumerable<EquipmentForWorkplace> equipmentForWorkplaces)
	{
		_context.EquipmentForWorkplace.RemoveRange(equipmentForWorkplaces);
	}

    public void AddRange(IEnumerable<EquipmentForWorkplace> equipmentForWorkplaces)
    {
        _context.EquipmentForWorkplace.AddRange(equipmentForWorkplaces);
    }

    public void Add(EquipmentForWorkplace equipmentForWorkplace)
	{
		_context.EquipmentForWorkplace.Add(equipmentForWorkplace);
	}

	public IEnumerable<EntityEntry> GetChangeTrackerEntries()
	{
		return _context.ChangeTracker.Entries();
	}

    public void ClearChangeTracker()
    {
        _context.ChangeTracker.Clear();
    }

    public void SaveChanges()
    {
		_context.SaveChanges();
    }
}
