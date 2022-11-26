using HotDesk.Entities;
using HotDesk.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HotDesk.Repositories;

public class ReservationRepository : IReservationRepository
{
	private readonly HotDeskDbContext _context;

	public ReservationRepository(HotDeskDbContext context)
	{
		_context = context;
	}

	public IEnumerable<Reservation> GetAllAfterDate(DateTime date)
	{
		return _context.Reservations
			.Where(r => r.TimeTo > date)
			.Include(r => r.Employee)
			.Include(r => r.Workplace);
	}

	public void Add(Reservation reservation)
	{
		_context.Add(reservation);
	}

	public Reservation GetById(int reservationId)
	{
		return _context.Reservations.FirstOrDefault(r => r.Id == reservationId);
	}
	public void Delete(Reservation reservation)
	{
		_context.Reservations.Remove(reservation);
	}

	public void SaveChanges()
	{
		_context.SaveChanges();
	}
}
