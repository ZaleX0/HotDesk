using HotDesk.Entities;

namespace HotDesk.Repositories.Interfaces;
public interface IReservationRepository
{
    void Add(Reservation reservation);
    void Delete(Reservation reservation);
    IEnumerable<Reservation> GetAllAfterDate(DateTime date);
    Reservation GetById(int reservationId);
    void SaveChanges();
}