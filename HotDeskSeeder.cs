using HotDesk.Entities;

namespace HotDesk;

public class HotDeskSeeder
{
	private readonly HotDeskDbContext _context;

	public HotDeskSeeder(HotDeskDbContext context)
	{
		_context = context;
	}

	public void Seed()
	{
		if (!_context.Database.CanConnect())
			return;

		if (!_context.Employees.Any())
		{
			var employees = GetEmployees();
			_context.Employees.AddRange(employees);
		}

        if (!_context.Workplaces.Any())
        {
            var workplaces = GetWorkplaces();
            _context.Workplaces.AddRange(workplaces);
        }

        if (!_context.Equipment.Any())
        {
            var equipment = GetEquipment();
            _context.Equipment.AddRange(equipment);
        }

        if (!_context.EquipmentForWorkplace.Any())
        {
            var equipmentForWorkplace = GetEquipmentForWorkplace();
            _context.EquipmentForWorkplace.AddRange(equipmentForWorkplace);
        }

        if (_context.ChangeTracker.HasChanges())
            _context.SaveChanges();
    }

    private IEnumerable<Employee> GetEmployees()
    {
        return new List<Employee>
        {
            new Employee { FirstName = "Employee", LastName = "One" },
            new Employee { FirstName = "Employee", LastName = "Two" },
            new Employee { FirstName = "Employee", LastName = "Three" }
        };
    }

    private IEnumerable<Workplace> GetWorkplaces()
	{
        return new List<Workplace>
        {
            new Workplace { Floor = 0, Room = 1, Table = 1 },
            new Workplace { Floor = 0, Room = 1, Table = 2 },
            new Workplace { Floor = 0, Room = 1, Table = 3 },
            new Workplace { Floor = 0, Room = 1, Table = 4 },

            new Workplace { Floor = 0, Room = 2, Table = 1 },
            new Workplace { Floor = 0, Room = 2, Table = 2 },
            new Workplace { Floor = 0, Room = 2, Table = 3 },
            new Workplace { Floor = 0, Room = 2, Table = 4 },

            new Workplace { Floor = 0, Room = 3, Table = 1 },
            new Workplace { Floor = 0, Room = 3, Table = 2 },
            new Workplace { Floor = 0, Room = 3, Table = 3 },
            new Workplace { Floor = 0, Room = 3, Table = 4 },

            new Workplace { Floor = 1, Room = 1, Table = 1 },
            new Workplace { Floor = 1, Room = 1, Table = 2 },
        };
	}

    private IEnumerable<Equipment> GetEquipment()
    {
        return new List<Equipment>
        {
            new Equipment { Type = "Monitor" },
            new Equipment { Type = "Keyboard" },
            new Equipment { Type = "Mouse" },
            new Equipment { Type = "Printer" }
        };
    }

    private IEnumerable<EquipmentForWorkplace> GetEquipmentForWorkplace()
    {
        return new List<EquipmentForWorkplace>
        {
            new EquipmentForWorkplace { WorkplaceId = 1, EquipmentId = 1, Count = 1 },
            new EquipmentForWorkplace { WorkplaceId = 1, EquipmentId = 3, Count = 1 },

            new EquipmentForWorkplace { WorkplaceId = 2, EquipmentId = 1, Count = 2 },
            new EquipmentForWorkplace { WorkplaceId = 2, EquipmentId = 2, Count = 1 },

            new EquipmentForWorkplace { WorkplaceId = 3, EquipmentId = 1, Count = 1 },

            new EquipmentForWorkplace { WorkplaceId = 5, EquipmentId = 1, Count = 2 },

            new EquipmentForWorkplace { WorkplaceId = 10, EquipmentId = 4, Count = 3 },
        };
    }
}
