using Microsoft.EntityFrameworkCore;

namespace HotDesk.Entities;

public class HotDeskDbContext : DbContext
{
	public HotDeskDbContext(DbContextOptions<HotDeskDbContext> options)
		: base(options)
	{
	}

	public DbSet<Employee> Employees { get; set; }
	public DbSet<Workplace> Workplaces { get; set; }
	public DbSet<Equipment> Equipment { get; set; }
	public DbSet<EquipmentForWorkplace> EquipmentForWorkplace { get; set; }
	public DbSet<Reservation> Reservations { get; set; }
}
