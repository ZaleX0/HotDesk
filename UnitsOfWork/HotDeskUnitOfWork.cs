using AutoMapper;
using HotDesk.Repositories.Interfaces;

namespace HotDesk.UnitsOfWork;

public class HotDeskUnitOfWork : IHotDeskUnitOfWork
{
    public HotDeskUnitOfWork(
        IEmployeesRepository employeesRepository,
        IWorkplaceRepository workplaceRepository,
        IEquipmentRepository equipmentRepository,
        IEquipmentForWorkplaceRepository equipmentForWorkplaceRepository,
        IReservationRepository reservationRepository)
    {
        Employees = employeesRepository;
        Workplaces = workplaceRepository;
        Equipment = equipmentRepository;
        EquipmentForWorkplace = equipmentForWorkplaceRepository;
        Reservations = reservationRepository;
    }

    public IEmployeesRepository Employees { get; set; }
    public IWorkplaceRepository Workplaces { get; set; }
    public IEquipmentRepository Equipment { get; set; }
    public IEquipmentForWorkplaceRepository EquipmentForWorkplace { get; set; }
    public IReservationRepository Reservations { get; set; }
}
