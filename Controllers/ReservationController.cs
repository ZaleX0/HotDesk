using HotDesk.DataTransferObjects;
using HotDesk.Services;
using Microsoft.AspNetCore.Mvc;

namespace HotDesk.Controllers;

public class ReservationController : Controller
{
	private readonly ReservationService _reservationService;

	public ReservationController(ReservationService reservationService)
	{
		_reservationService = reservationService;
	}

	[HttpGet]
	public IActionResult Index(GetReservationInfoDto dto)
	{
		var model = _reservationService.GetReservationViewModel(dto);
		return View(model);
	}

	[HttpPost]
	public IActionResult Create(CreateReservationDto dto)
	{
		_reservationService.CreateReservation(dto);
		return View();
	}
}
