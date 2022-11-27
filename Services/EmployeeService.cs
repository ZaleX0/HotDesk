using AutoMapper;
using HotDesk.DataTransferObjects;
using HotDesk.Entities;
using HotDesk.UnitsOfWork;

namespace HotDesk.Services;

public class EmployeeService : IEmployeeService
{
	private readonly IHotDeskUnitOfWork _unitOfWork;
	private readonly IMapper _mapper;

	public EmployeeService(IHotDeskUnitOfWork unitOfWork, IMapper mapper)
	{
		_unitOfWork = unitOfWork;
		_mapper = mapper;
	}

	public void AddEmployee(AddEmployeeDto dto)
	{
		var employee = _mapper.Map<Employee>(dto);
		_unitOfWork.Employees.Add(employee);
	}
}
