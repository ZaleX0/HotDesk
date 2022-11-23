using AutoMapper;
using HotDesk.DataTransferObjects;
using HotDesk.Entities;

namespace HotDesk.MappingProfiles;

public class HotDeskMappingProfile : Profile
{
	public HotDeskMappingProfile()
	{
		CreateMap<Employee, EmployeeDto>()
			.ForMember(d => d.Name, c => c.MapFrom(s => $"{s.FirstName} {s.LastName}"));


		CreateMap<EquipmentForWorkplace, EquipmentForWorkplaceDto>()
			.ForMember(d => d.Type, c => c.MapFrom(s => s.Equipment.Type));

		CreateMap<EquipmentForWorkplace, EquipmentDto>()
			.ForMember(d => d.Id, c => c.MapFrom(s => s.Equipment.Id))
			.ForMember(d => d.Type, c => c.MapFrom(s => s.Equipment.Type))
			.ForMember(d => d.Count, c => c.MapFrom(s => s.Count));

        CreateMap<Equipment, EquipmentDto>();
		CreateMap<Workplace, WorkplaceDto>();

		CreateMap<CreateReservationDto, Reservation>()
			.ForMember(d => d.TimeFrom, c => c.MapFrom(s => s.From))
			.ForMember(d => d.TimeTo, c => c.MapFrom(s => s.To));
	}
}
