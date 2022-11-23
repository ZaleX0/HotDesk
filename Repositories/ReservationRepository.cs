using HotDesk.Entities;

namespace HotDesk.Repositories;

public class ReservationRepository
{
	private readonly HotDeskDbContext _context;

	public ReservationRepository(HotDeskDbContext context)
	{
		_context = context;
	}

	public List<Reservation> GetIntersecting(DateTime from, DateTime to)
	{
        return _context.Reservations
			.Where(r => r.TimeTo < from && r.TimeFrom < to)
			.ToList();
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
