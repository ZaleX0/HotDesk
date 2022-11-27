using HotDesk.Entities;

namespace HotDesk.Repositories.Interfaces;
public interface IEmployeesRepository
{
    IEnumerable<Employee> GetAll();
    void Add(Employee employee);
}