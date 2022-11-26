using HotDesk.Entities;
using HotDesk.Repositories.Interfaces;

namespace HotDesk.Repositories;

public class EmployeesRepository : IEmployeesRepository
{
	private readonly HotDeskDbContext _context;

	public EmployeesRepository(HotDeskDbContext context)
	{
		_context = context;
	}

	public IEnumerable<Employee> GetAll()
	{
		return _context.Employees.ToList();
	}
}
