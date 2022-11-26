using HotDesk.Entities;

namespace HotDesk.Repositories.Interfaces;
public interface IEquipmentRepository
{
    IEnumerable<Equipment> GetAll();
}