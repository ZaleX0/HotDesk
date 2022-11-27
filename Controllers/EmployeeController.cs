using HotDesk.DataTransferObjects;
using HotDesk.Services;
using Microsoft.AspNetCore.Mvc;

namespace HotDesk.Controllers;
public class EmployeeController : Controller
{
    private readonly IEmployeeService _service;

    public EmployeeController(IEmployeeService service)
    {
        _service = service;
    }

    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Add(AddEmployeeDto dto)
    {
        _service.AddEmployee(dto);
        return RedirectToAction("Index");
    }
}
