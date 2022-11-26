using HotDesk.Entities;
using HotDesk.Repositories.Interfaces;

namespace HotDesk.Repositories;

public class EquipmentRepository : IEquipmentRepository
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
