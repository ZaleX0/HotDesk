using HotDesk.Entities;
using HotDesk.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HotDesk.Repositories;

public class WorkplaceRepository : IWorkplaceRepository
{
	private readonly HotDeskDbContext _context;

	public WorkplaceRepository(HotDeskDbContext context)
	{
		_context = context;
	}

	public IEnumerable<Workplace> GetAll()
	{
		return _context.Workplaces.ToList();
	}

	public Workplace? GetById(int workplaceId)
	{
		return _context.Workplaces.FirstOrDefault(w => w.Id == workplaceId);
	}
}
