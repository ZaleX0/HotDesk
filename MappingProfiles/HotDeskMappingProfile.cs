using AutoMapper;
using HotDesk.DataTransferObjects;
using HotDesk.Entities;

namespace HotDesk.MappingProfiles;

public class HotDeskMappingProfile : Profile
{
	public HotDeskMappingProfile()
	{
		CreateMap<AddEmployeeDto, Employee>();
		CreateMap<Employee, EmployeeDto>()
			.ForMember(d => d.Name, c => c.MapFrom(s => $"{s.FirstName} {s.LastName}"));

        CreateMap<EquipmentForWorkplaceDto, EquipmentForWorkplace>();
        CreateMap<EquipmentForWorkplace, EquipmentForWorkplaceDto>()
			.ForMember(d => d.Type, c => c.MapFrom(s => s.Equipment.Type));

		CreateMap<EquipmentForWorkplace, EquipmentDto>()
			.ForMember(d => d.Id, c => c.MapFrom(s => s.Equipment.Id))
			.ForMember(d => d.Type, c => c.MapFrom(s => s.Equipment.Type))
			.ForMember(d => d.Count, c => c.MapFrom(s => s.Count));

		CreateMap<AddEquipmentForWorkplaceDto, EquipmentForWorkplace>();
        CreateMap<Equipment, EquipmentDto>();
		CreateMap<Workplace, WorkplaceDto>();

		CreateMap<CreateReservationDto, Reservation>()
			.ForMember(d => d.TimeFrom, c => c.MapFrom(s => s.From))
			.ForMember(d => d.TimeTo, c => c.MapFrom(s => s.To));

		CreateMap<Reservation, ReservationDto>()
			.ForMember(d => d.EmployeeName, c => c.MapFrom(s => $"{s.Employee.FirstName} {s.Employee.LastName}"))
			.ForMember(d => d.Floor, c => c.MapFrom(s => s.Workplace.Floor))
			.ForMember(d => d.Room, c => c.MapFrom(s => s.Workplace.Room))
			.ForMember(d => d.Table, c => c.MapFrom(s => s.Workplace.Table));
    }
}
