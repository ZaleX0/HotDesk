﻿using HotDesk.DataTransferObjects;
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
}
