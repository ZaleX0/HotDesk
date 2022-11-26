using HotDesk.Repositories.Interfaces;

namespace HotDesk.UnitsOfWork;
public interface IHotDeskUnitOfWork
{
    IEmployeesRepository Employees { get; set; }
    IEquipmentRepository Equipment { get; set; }
    IEquipmentForWorkplaceRepository EquipmentForWorkplace { get; set; }
    IReservationRepository Reservations { get; set; }
    IWorkplaceRepository Workplaces { get; set; }
}