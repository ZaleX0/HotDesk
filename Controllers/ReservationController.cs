using HotDesk.DataTransferObjects;
using HotDesk.Services;
using Microsoft.AspNetCore.Mvc;

namespace HotDesk.Controllers;

public class ReservationController : Controller
{
	private readonly IReservationService _reservationService;

	public ReservationController(IReservationService reservationService)
	{
		_reservationService = reservationService;
	}

    public IActionResult Index()
    {
        var model = _reservationService.GetReservationViewModel();
        return View(model);
    }

    [HttpGet]
	public IActionResult Create(GetReservationInfoDto dto)
	{
		var model = _reservationService.GetCreateReservationViewModel(dto);
		return View(model);
	}

    [HttpPost]
    public IActionResult Create(CreateReservationDto dto)
    {
        _reservationService.CreateReservation(dto);
        return RedirectToAction("Index");
    }

    [HttpPost]
    public IActionResult Delete(int reservationId)
    {
        _reservationService.DeleteReservation(reservationId);
        return RedirectToAction("Index");
    }
}
