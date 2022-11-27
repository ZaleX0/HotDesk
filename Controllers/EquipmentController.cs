using HotDesk.DataTransferObjects;
using HotDesk.Services;
using Microsoft.AspNetCore.Mvc;

namespace HotDesk.Controllers;
public class EquipmentController : Controller
{
    private readonly IEquipmentService _service;

    public EquipmentController(IEquipmentService service)
    {
        _service = service;
    }

    [HttpGet]
    public IActionResult Index()
    {
        var model = _service.GetEquipmentForWorkplaceViewModel();
        return View(model);
    }

    [HttpPost]
    public IActionResult Add(AddEquipmentForWorkplaceDto dto)
    {
        _service.AddEquipmentToWorkplace(dto);
        return RedirectToAction("Index");
    }
}
