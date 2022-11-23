using AutoMapper;
using HotDesk.DataTransferObjects;
using HotDesk.Entities;
using HotDesk.Models;
using HotDesk.Repositories;
using System;
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



	public ReservationViewModel GetReservationViewModel(GetReservationInfoDto dto)
	{
		if (dto.From < DateTime.Now)
		{
            dto.From = DateTime.Now.Date.AddDays(1).AddHours(8);
			dto.To = dto.From.AddHours(8);
		}

		var model = new ReservationViewModel
		{
            SelectedEmployeeId = dto.EmployeeId,
            SelectedWorkplaceId = dto.WorkplaceId,
			TimeFrom = dto.From,
			TimeTo = dto.To,
            Employees = GetEmployees(),
			Workplaces = GetAvailableWorkplaces(dto.From, dto.To),
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
	private List<WorkplaceDto> GetAvailableWorkplaces(DateTime from, DateTime to)
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
		// add reservation
		var reservation = _mapper.Map<Reservation>(dto);
		_reservationRepository.Add(reservation);


        var workplaces = GetAvailableWorkplaces(dto.From, dto.To);
		var targetWp = workplaces.First(w => w.Id == reservation.WorkplaceId);
        
		
		

        foreach (var equipment in dto.AdditionalEquipment)
		{	// for every eq
			for (int i = 0; i < equipment.Count; i++)
			{
				// find closest (table, room, floor) by eqId
				// 1. from same room
				var test = workplaces.Where(w => w.Id != targetWp.Id
					&& w.Floor == targetWp.Floor
					&& w.Room == targetWp.Room
					&& w.Equipment.Any(e => e.EquipmentId == equipment.Id))
					.Aggregate((t1, t2) => Math.Abs(t1.Table - targetWp.Table) < Math.Abs(t2.Table - targetWp.Table) ? t1 : t2);

                // add 1 to reserved eqForWp count
                // substract 1 from closest eqForWp
            }
		}
		
        _equipmentForWorkplaceRepository.Update();

		// save changes
		//_reservationRepository.SaveChanges();
		//_equipmentForWorkplaceRepository.SaveChanges();
	}
}
