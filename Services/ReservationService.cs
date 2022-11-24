using AutoMapper;
using HotDesk.DataTransferObjects;
using HotDesk.Entities;
using HotDesk.Models;
using HotDesk.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using System;
using System.ComponentModel;
using System.Linq;
using System.Security.Cryptography.Xml;

namespace HotDesk.Services;

public class ReservationService
{
	private readonly IMapper _mapper;
	private readonly EmployeesRepository _employeesRepository;
	private readonly WorkplaceRepository _workplaceRepository;
	private readonly EquipmentRepository _equipmentRepository;
	private readonly EquipmentForWorkplaceRepository _equipmentForWorkplaceRepository;
	private readonly ReservationRepository _reservationRepository;

	public ReservationService(
		IMapper mapper,
		EmployeesRepository employeesRepository,
		WorkplaceRepository workplaceRepository,
		EquipmentRepository equipmentRepository,
		EquipmentForWorkplaceRepository equipmentForWorkplaceRepository,
		ReservationRepository reservationRepository)
	{
		_mapper = mapper;
		_employeesRepository = employeesRepository;
		_workplaceRepository = workplaceRepository;
		_equipmentRepository = equipmentRepository;
		_equipmentForWorkplaceRepository = equipmentForWorkplaceRepository;
		_reservationRepository = reservationRepository;
	}



	public CreateReservationViewModel GetCreateReservationViewModel(GetReservationInfoDto dto)
	{
		if (dto.From < DateTime.Now)
		{
            dto.From = DateTime.Now.Date.AddDays(1).AddHours(8);
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
        var employees = _employeesRepository.GetAll();
        return _mapper.Map<List<EmployeeDto>>(employees);
    }
	private List<WorkplaceDto> GetWorkplaces(DateTime from, DateTime to)
	{
        var workplaces = _workplaceRepository.GetAll();
        var equipmentForWorkplaces = _equipmentForWorkplaceRepository.GetAll();
		var intersectingReservations = _reservationRepository.GetIntersecting(from, to);

        var workplacesDtos = _mapper.Map<List<WorkplaceDto>>(workplaces);
        var eqForWpDtos = _mapper.Map<List<EquipmentForWorkplaceDto>>(equipmentForWorkplaces);

        foreach (var wpDto in workplacesDtos)
        {
            wpDto.Equipment = eqForWpDtos.Where(e => e.WorkplaceId == wpDto.Id).ToList();

			// if intersecting reservation exists that means workplace is reserved
			wpDto.IsReserved = intersectingReservations.Any(r => r.WorkplaceId == wpDto.Id);
        }

		return workplacesDtos;
    }
    private WorkplaceDto GetWorkplace(int workplaceId)
	{
		var workplace = _workplaceRepository.GetById(workplaceId);
        var equipmentForWorkplace = _equipmentForWorkplaceRepository.GetForWorkplace(workplaceId);

        var workplaceDto = _mapper.Map<WorkplaceDto>(workplace);
        var eqForWpDto = _mapper.Map<List<EquipmentForWorkplaceDto>>(equipmentForWorkplace);

		if (workplaceDto != null && eqForWpDto != null)
			workplaceDto.Equipment = eqForWpDto;


        return workplaceDto;
	}
	private List<EquipmentDto> GetAvailableEquipment(DateTime from, DateTime to, int selectedWpId)
	{
        var equipmentForWorkplaces = _equipmentForWorkplaceRepository.GetAll();
		var intersectingReservations = _reservationRepository.GetIntersecting(from, to);

        // creating temporary EquipmentDto list
        var eqCounts = equipmentForWorkplaces
			// exclude reserved workplaces and currently selected workplace in the view
			.Where(e => !intersectingReservations.Any(r => r.WorkplaceId == e.WorkplaceId) && e.WorkplaceId != selectedWpId)
			.GroupBy(e => e.EquipmentId)
			.Select(g => new EquipmentDto
            {
				Id = g.Key,
				Count = g.Sum(e => e.Count)
			});

		// gets all equipment types
        var equipment = _equipmentRepository.GetAll();
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
		var reservedWp = workplaces.First(w => w.Id == dto.WorkplaceId);

        foreach (var equipment in dto.AdditionalEquipment)
		{	// for every eq
			for (int i = 0; i < equipment.Count; i++)
			{
				// workplaces without reserved workplace and only with desired equipment type
				var workplacesWithEqType = workplaces.Where(w => w.Id != reservedWp.Id
					&& w.Equipment.Any(e => e.EquipmentId == equipment.Id && e.Count != 0));

				// find closest (table, room, floor)
				// 1. from same room
				var wpList = workplacesWithEqType.Where(w => w.Floor == reservedWp.Floor && w.Room == reservedWp.Room);
                if (!wpList.IsNullOrEmpty())
				{
					var resultWp = wpList.Aggregate((t1, t2) => Math.Abs(t1.Table - reservedWp.Table) <= Math.Abs(t2.Table - reservedWp.Table) ? t1 : t2);
                    MoveEquipment(workplaces, equipment.Id, resultWp, reservedWp);
                    continue;
				}

                // 2. from same floor but different room
                wpList = workplacesWithEqType.Where(w => w.Floor == reservedWp.Floor);
				if (!wpList.IsNullOrEmpty())
				{	
					var resultWp = wpList.Aggregate((r1, r2) => Math.Abs(r1.Room - reservedWp.Room) <= Math.Abs(r2.Room - reservedWp.Room) ? r1 : r2);
                    MoveEquipment(workplaces, equipment.Id, resultWp, reservedWp);
                    continue;
				}

				// 3. from different floor
				wpList = workplacesWithEqType;
				if (!wpList.IsNullOrEmpty())
				{
                    var resultWp = wpList.Aggregate((f1, f2) => Math.Abs(f1.Floor - reservedWp.Floor) <= Math.Abs(f2.Floor - reservedWp.Floor) ? f1 : f2);
                    MoveEquipment(workplaces, equipment.Id, resultWp, reservedWp);
                }
            }
		}

		UpdateDatabase(workplaces, dto);
	}

    private void MoveEquipment(List<WorkplaceDto> workplaces, int eqId, WorkplaceDto source, WorkplaceDto destination)
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
			workplaces.FirstOrDefault(w => w.Id == destination.Id).Equipment.Add(destinationEq);
        }
        else
        {
            destinationEq.Count++;
        }
    }

	private void UpdateDatabase(List<WorkplaceDto> workplaceDtos, CreateReservationDto createReservationDto)
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
        _equipmentForWorkplaceRepository.ClearChangeTracker();

        _equipmentForWorkplaceRepository.AddRange(createList);
        _equipmentForWorkplaceRepository.RemoveRange(removeList);
        _equipmentForWorkplaceRepository.UpdateRange(updateList);

        // add reservation
        var reservation = _mapper.Map<Reservation>(createReservationDto);
        _reservationRepository.Add(reservation);

        // save changes
        _reservationRepository.SaveChanges();
        _equipmentForWorkplaceRepository.SaveChanges();
    }



	public ReservationViewModel GetReservationViewModel()
	{
		var reservations = _reservationRepository.GetAll(DateTime.Now);
		var reservationDtos = _mapper.Map<IEnumerable<ReservationDto>>(reservations);
		return new ReservationViewModel { Reservations = reservationDtos.ToList() };
	}
}
