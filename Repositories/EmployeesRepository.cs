using HotDesk.Entities;

namespace HotDesk.Repositories;

public class EmployeesRepository
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
