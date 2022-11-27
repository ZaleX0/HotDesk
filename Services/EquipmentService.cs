using AutoMapper;
using HotDesk.DataTransferObjects;
using HotDesk.Entities;
using HotDesk.Models;
using HotDesk.UnitsOfWork;

namespace HotDesk.Services;

public class EquipmentService : IEquipmentService
{
	private readonly IHotDeskUnitOfWork _unitOfWork;
	private readonly IMapper _mapper;

	public EquipmentService(IHotDeskUnitOfWork unitOfWork, IMapper mapper)
	{
		_unitOfWork = unitOfWork;
		_mapper = mapper;
	}

	public EquipmentForWorkplaceViewModel GetEquipmentForWorkplaceViewModel()
	{
		var workplaces = _unitOfWork.Workplaces.GetAll();
		var workplaceDtos = _mapper.Map<List<WorkplaceDto>>(workplaces);

		var equipment = _unitOfWork.Equipment.GetAll();
        var equipmentDtos = _mapper.Map<List<EquipmentDto>>(equipment);

		var model = new EquipmentForWorkplaceViewModel
		{
			Workplaces = workplaceDtos,
			Equipment = equipmentDtos
		};
		return model;
    }

	public void AddEquipmentToWorkplace(AddEquipmentForWorkplaceDto dto)
	{
		var eqforwp = _mapper.Map<EquipmentForWorkplace>(dto);
		_unitOfWork.EquipmentForWorkplace.Add(eqforwp);
		_unitOfWork.EquipmentForWorkplace.SaveChanges();
	}
}
