using HotDesk.Entities;
using Microsoft.EntityFrameworkCore;

namespace HotDesk.Repositories;

public class ReservationRepository
{
	private readonly HotDeskDbContext _context;

	public ReservationRepository(HotDeskDbContext context)
	{
		_context = context;
	}

	public IEnumerable<Reservation> GetAll(DateTime TimeTo)
	{
		return _context.Reservations
			.Where(r => r.TimeTo > TimeTo)
			.Include(r=>r.Employee)
			.Include(r=>r.Workplace);
	}

	public IEnumerable<Reservation> GetIntersecting(DateTime TimeFrom, DateTime TimeTo)
	{
		return _context.Reservations.Where(r => r.TimeFrom < TimeTo && TimeFrom < r.TimeTo);
	}

	public void Add(Reservation reservation)
	{
		_context.Add(reservation);
	}

	public void SaveChanges()
	{
		_context.SaveChanges();
	}
}
