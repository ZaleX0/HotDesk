using HotDesk.Entities;

namespace HotDesk.Repositories.Interfaces;
public interface IWorkplaceRepository
{
    IEnumerable<Workplace> GetAll();
    Workplace? GetById(int workplaceId);
}