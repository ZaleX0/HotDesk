using HotDesk.Entities;

namespace HotDesk.Repositories;

public class EquipmentRepository
{
	private readonly HotDeskDbContext _context;

	public EquipmentRepository(HotDeskDbContext context)
	{
		_context = context;
	}

	public IEnumerable<Equipment> GetAll()
	{
		return _context.Equipment.ToList();
	}
}
