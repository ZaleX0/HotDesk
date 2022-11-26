using AutoMapper;
using HotDesk.DataTransferObjects;
using HotDesk.Entities;
using HotDesk.Models;
using HotDesk.Repositories.Interfaces;
using HotDesk.UnitsOfWork;
using Microsoft.IdentityModel.Tokens;

namespace HotDesk.Services;

public class ReservationService : IReservationService
{
	private readonly IMapper _mapper;
	private readonly IHotDeskUnitOfWork _unitOfWork;

	public ReservationService(IMapper mapper, IHotDeskUnitOfWork hotDeskUnitOfWork)
	{
		_mapper = mapper;
		_unitOfWork = hotDeskUnitOfWork;
	}



	public CreateReservationViewModel GetCreateReservationViewModel(GetReservationInfoDto dto)
	{
		if (dto.From < DateTime.Now)
		{
			dto.From = DateTime.Now.Date.AddDays(1).AddHours(8);
			dto.To = dto.From.AddHours(8);
		}

		if (dto.From > dto.To)
		{
			dto.To = dto.From.AddHours(8);
		}

		var model = new CreateReservationViewModel
		{
			SelectedEmployeeId = dto.EmployeeId,
			SelectedWorkplaceId = dto.WorkplaceId,
			TimeFrom = dto.From,
			TimeTo = dto.To,
			Employees = GetEmployees(),
			Workplaces = GetWorkplaces(dto.From, dto.To),
			SelectedWorkplace = GetWorkplace(dto.WorkplaceId),
			AvailableEquipment = GetAvailableEquipment(dto.From, dto.To, dto.WorkplaceId)
		};
		return model;
	}

	private List<EmployeeDto> GetEmployees()
	{
		var employees = _unitOfWork.Employees.GetAll();
		return _mapper.Map<List<EmployeeDto>>(employees);
	}
	private List<WorkplaceDto> GetWorkplaces(DateTime from, DateTime to)
	{
		var workplaces = _unitOfWork.Workplaces.GetAll();
		var equipmentForWorkplaces = _unitOfWork.EquipmentForWorkplace.GetAll();

		var reservations = _unitOfWork.Reservations.GetAllAfterDate(DateTime.Now).ToList();
		var overlapingReservations = GetOverlapingReservations(reservations, from, to);

		var workplacesDtos = _mapper.Map<List<WorkplaceDto>>(workplaces);
		var eqForWpDtos = _mapper.Map<List<EquipmentForWorkplaceDto>>(equipmentForWorkplaces);

		foreach (var wpDto in workplacesDtos)
		{
			wpDto.Equipment = eqForWpDtos.Where(e => e.WorkplaceId == wpDto.Id).ToList();

			// if overlaping reservation exists that means workplace is reserved
			wpDto.IsReserved = overlapingReservations.Any(r => r.WorkplaceId == wpDto.Id);

			var reservationForWp = reservations.FirstOrDefault(r => r.WorkplaceId == wpDto.Id);
			if (reservationForWp != null)
			{
				wpDto.IsReservedInFuture = reservationForWp.TimeFrom >= DateTime.Now;
			}
		}

		return workplacesDtos;
	}
	private WorkplaceDto GetWorkplace(int workplaceId)
	{
		var workplace = _unitOfWork.Workplaces.GetById(workplaceId);
		var equipmentForWorkplace = _unitOfWork.EquipmentForWorkplace.GetForWorkplace(workplaceId);

		var workplaceDto = _mapper.Map<WorkplaceDto>(workplace);
		var eqForWpDto = _mapper.Map<List<EquipmentForWorkplaceDto>>(equipmentForWorkplace);

		if (workplaceDto != null && eqForWpDto != null)
		{
			workplaceDto.Equipment = eqForWpDto;

			var reservations = _unitOfWork.Reservations.GetAllAfterDate(DateTime.Now).ToList();
			var reservationForWp = reservations.FirstOrDefault(r => r.WorkplaceId == workplaceDto.Id);
			if (reservationForWp != null)
			{
				workplaceDto.IsReservedInFuture = reservationForWp.TimeFrom >= DateTime.Now;
			}
		}

		return workplaceDto;
	}
	private List<Reservation> GetOverlapingReservations(List<Reservation> reservations, DateTime TimeFrom, DateTime TimeTo)
	{
		return reservations.Where(r => r.TimeFrom < TimeTo && TimeFrom < r.TimeTo).ToList();
	}
	private List<EquipmentDto> GetAvailableEquipment(DateTime from, DateTime to, int selectedWpId)
	{
		// get workplaces that are not reserved now and in the future
		var workplaces = GetWorkplaces(from, to)
			.Where(w => w.Id != selectedWpId
			&& !w.IsReserved
			&& !w.IsReservedInFuture);

		// sum equipment Count per equipment type
		var eqCounts = workplaces.SelectMany(w => w.Equipment)
			.GroupBy(e => e.EquipmentId)
			.Select(g => new EquipmentDto
			{
				Id = g.Key,
				Count = g.Sum(e => e.Count)
			});

		// gets all equipment types
		var equipment = _unitOfWork.Equipment.GetAll();
		var equipmentDtos = _mapper.Map<List<EquipmentDto>>(equipment);

		// merge objects
		foreach (var eqDto in equipmentDtos)
		{
			var eqCount = eqCounts.FirstOrDefault(e => e.Id == eqDto.Id);
			if (eqCount != null)
				eqDto.Count = eqCount.Count;
			else
				eqDto.Count = 0;
		}

		return equipmentDtos;
	}

	public void CreateReservation(CreateReservationDto dto)
	{
		var workplaces = GetWorkplaces(dto.From, dto.To).Where(w => !w.IsReserved).ToList();
		var wpForReservation = workplaces.First(w => w.Id == dto.WorkplaceId);

		if (wpForReservation.IsReservedInFuture)
		{
			UpdateDatabase(dto);
			return;
		}

		if (dto.AdditionalEquipment.IsNullOrEmpty())
		{
			UpdateDatabase(dto);
			return;
		}

		foreach (var equipment in dto.AdditionalEquipment)
		{   // for every eq
			for (int i = 0; i < equipment.Count; i++)
			{
				// workplaces: not reserved, not reserved in the future and only with desired equipment type
				var workplacesWithEqType = workplaces.Where(w => w.Id != wpForReservation.Id
					&& !w.IsReservedInFuture
					&& w.Equipment.Any(e => e.EquipmentId == equipment.Id && e.Count != 0));

				// find closest (table, room, floor)
				// 1. from same room but different table
				var wpList = workplacesWithEqType.Where(w => w.Floor == wpForReservation.Floor && w.Room == wpForReservation.Room);
				if (!wpList.IsNullOrEmpty())
				{
					var resultWp = wpList.Aggregate((t1, t2) => Math.Abs(t1.Table - wpForReservation.Table) <= Math.Abs(t2.Table - wpForReservation.Table) ? t1 : t2);
					MoveEquipment(workplaces, equipment.Id, resultWp, wpForReservation);
					continue;
				}

				// 2. from same floor but different room
				wpList = workplacesWithEqType.Where(w => w.Floor == wpForReservation.Floor);
				if (!wpList.IsNullOrEmpty())
				{
					var resultWp = wpList.Aggregate((r1, r2) => Math.Abs(r1.Room - wpForReservation.Room) <= Math.Abs(r2.Room - wpForReservation.Room) ? r1 : r2);
					MoveEquipment(workplaces, equipment.Id, resultWp, wpForReservation);
					continue;
				}

				// 3. from different floor
				wpList = workplacesWithEqType;
				if (!wpList.IsNullOrEmpty())
				{
					var resultWp = wpList.Aggregate((f1, f2) => Math.Abs(f1.Floor - wpForReservation.Floor) <= Math.Abs(f2.Floor - wpForReservation.Floor) ? f1 : f2);
					MoveEquipment(workplaces, equipment.Id, resultWp, wpForReservation);
				}
			}
		}

		UpdateDatabase(dto, workplaces);
	}
	private void MoveEquipment(List<WorkplaceDto> workplaceDtos, int eqId, WorkplaceDto source, WorkplaceDto destination)
	{
		// substract 1 from closest eqforwp
		var sourceEq = source.Equipment.FirstOrDefault(e => e.EquipmentId == eqId);

		sourceEq.Count--;

		// add 1 to reserved eqforwp count
		var destinationEq = destination.Equipment.FirstOrDefault(e => e.EquipmentId == eqId);
		if (destinationEq == null)
		{
			destinationEq = new EquipmentForWorkplaceDto
			{
				WorkplaceId = destination.Id,
				EquipmentId = eqId,
				Count = 1
			};
			workplaceDtos.FirstOrDefault(w => w.Id == destination.Id).Equipment.Add(destinationEq);
		}
		else
		{
			destinationEq.Count++;
		}
	}
	private void UpdateDatabase(CreateReservationDto createReservationDto)
	{
		var reservation = _mapper.Map<Reservation>(createReservationDto);
        _unitOfWork.Reservations.Add(reservation);
		_unitOfWork.Reservations.SaveChanges();
	}
	private void UpdateDatabase(CreateReservationDto createReservationDto, List<WorkplaceDto> workplaceDtos)
	{
		var createListDto = new List<EquipmentForWorkplaceDto>();
		var removeListDto = new List<EquipmentForWorkplaceDto>();
		var updateListDto = new List<EquipmentForWorkplaceDto>();
		foreach (var wp in workplaceDtos)
		{
			createListDto.AddRange(wp.Equipment.Where(e => e.Id == 0).ToList());
			removeListDto.AddRange(wp.Equipment.Where(e => e.Count == 0).ToList());
			updateListDto.AddRange(wp.Equipment.Where(e => e.Id != 0 && e.Count != 0).ToList());
		}
		var createList = _mapper.Map<List<EquipmentForWorkplace>>(createListDto);
		var removeList = _mapper.Map<List<EquipmentForWorkplace>>(removeListDto);
		var updateList = _mapper.Map<List<EquipmentForWorkplace>>(updateListDto);

        // this is needed because it tracks some changes while searching for the closest workplace
        _unitOfWork.EquipmentForWorkplace.ClearChangeTracker();

        _unitOfWork.EquipmentForWorkplace.AddRange(createList);
        _unitOfWork.EquipmentForWorkplace.RemoveRange(removeList);
        _unitOfWork.EquipmentForWorkplace.UpdateRange(updateList);

		// add reservation
		var reservation = _mapper.Map<Reservation>(createReservationDto);
        _unitOfWork.Reservations.Add(reservation);

        // save changes
        _unitOfWork.Reservations.SaveChanges();
        _unitOfWork.EquipmentForWorkplace.SaveChanges();
	}

	public ReservationViewModel GetReservationViewModel()
	{
		var reservations = _unitOfWork.Reservations.GetAllAfterDate(DateTime.Now);
		var reservationDtos = _mapper.Map<IEnumerable<ReservationDto>>(reservations);
		return new ReservationViewModel { Reservations = reservationDtos.ToList() };
	}

	public void DeleteReservation(int reservationId)
	{
		var reservation = _unitOfWork.Reservations.GetById(reservationId);
        _unitOfWork.Reservations.Delete(reservation);
        _unitOfWork.Reservations.SaveChanges();
	}
}
