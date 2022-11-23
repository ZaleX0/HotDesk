using HotDesk.Entities;
using Microsoft.EntityFrameworkCore;

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

	public void Update()
	{
		// TODO: Update db context
	}

    public void SaveChanges()
    {
		_context.SaveChanges();
    }
}
